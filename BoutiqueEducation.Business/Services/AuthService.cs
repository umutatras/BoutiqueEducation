using BoutiqueEducation.Business.Interfaces.Services;
using BoutiqueEducation.Business.Models;
using BoutiqueEducation.Business.Models.DTOs;
using BoutiqueEducation.Business.Settings;
using BoutiqueEducation.Entity.Entities;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace BoutiqueEducation.Business.Services;

public sealed class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly ITokenService _tokenService;
    private readonly JwtSettings _jwtSettings;
    private readonly GoogleAuthSettings _googleSettings;

    public AuthService(
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager,
        ITokenService tokenService,
        IOptions<JwtSettings> jwtSettings,
        IOptions<GoogleAuthSettings> googleSettings)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _tokenService = tokenService;
        _jwtSettings = jwtSettings.Value;
        _googleSettings = googleSettings.Value;
    }

    public async Task<Result> RegisterAsync(RegisterDto dto)
    {
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
            return Result.Failure("Bu e-posta adresi zaten kullanılıyor.");

        var user = new AppUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FullName = dto.FullName,
            IsApproved = false // Admin onayı bekleyecek
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
            return Result.Failure("Kullanıcı oluşturulamadı: " + string.Join(", ", result.Errors.Select(e => e.Description)));

        // Yeni kayıt olan kullanıcı "Uye" rolü alır
        await EnsureRoleAsync("Uye");
        await _userManager.AddToRoleAsync(user, "Uye");

        return Result.Success();
    }

    public async Task<Result<TokenResponseDto>> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            return Result<TokenResponseDto>.Failure("E-posta veya şifre hatalı.");

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!isPasswordValid)
            return Result<TokenResponseDto>.Failure("E-posta veya şifre hatalı.");

        return Result<TokenResponseDto>.Success(await BuildTokenResponse(user));
    }

    public async Task<Result<TokenResponseDto>> LoginWithGoogleAsync(GoogleLoginDto dto)
    {
        try
        {
            // Google Id Token'ı doğrula — ClientId ile eşleşmeli
            var validationSettings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = [_googleSettings.ClientId]
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(dto.IdToken, validationSettings);

            var user = await _userManager.FindByEmailAsync(payload.Email);

            if (user == null)
            {
                // İlk kez giriş → Otomatik kayıt
                user = new AppUser
                {
                    UserName = payload.Email,
                    Email = payload.Email,
                    FullName = payload.Name,
                    GoogleProviderId = payload.Subject,
                    EmailConfirmed = true  // Google doğruladı
                };

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                    return Result<TokenResponseDto>.Failure("Google ile kayıt olurken hata oluştu.");

                await EnsureRoleAsync("Student");
                await _userManager.AddToRoleAsync(user, "Student");
            }
            else if (string.IsNullOrEmpty(user.GoogleProviderId))
            {
                // Daha önce şifreyle kayıt → Google ID'yi bağla
                user.GoogleProviderId = payload.Subject;
                await _userManager.UpdateAsync(user);
            }

            return Result<TokenResponseDto>.Success(await BuildTokenResponse(user));
        }
        catch (InvalidJwtException)
        {
            return Result<TokenResponseDto>.Failure("Geçersiz Google yetkilendirmesi.");
        }
    }

    // ─── Private Helpers ───────────────────────────────────────

    private async Task EnsureRoleAsync(string roleName)
    {
        if (!await _roleManager.RoleExistsAsync(roleName))
            await _roleManager.CreateAsync(new AppRole { Name = roleName });
    }

    private async Task<TokenResponseDto> BuildTokenResponse(AppUser user)
    {
        var tokenString = await _tokenService.GenerateJwtTokenAsync(user);
        return new TokenResponseDto
        {
            AccessToken = tokenString,
            Expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes)
        };
    }
}
