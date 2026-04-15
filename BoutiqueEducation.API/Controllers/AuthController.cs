using BoutiqueEducation.Business.Interfaces.Services;
using BoutiqueEducation.Business.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BoutiqueEducation.API.Controllers;

[Route("api/auth")]
public sealed class AuthController : BaseController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        return HandleResult(await _authService.RegisterAsync(dto), "Kullanıcı başarıyla oluşturuldu.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        return HandleResult(await _authService.LoginAsync(dto));
    }

    [HttpPost("google-login")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto dto)
    {
        return HandleResult(await _authService.LoginWithGoogleAsync(dto));
    }
}
