using BoutiqueEducation.Business.Models;
using BoutiqueEducation.Business.Models.DTOs;

namespace BoutiqueEducation.Business.Interfaces.Services;

public interface IAuthService
{
    Task<Result<TokenResponseDto>> LoginWithGoogleAsync(GoogleLoginDto dto);
    Task<Result> RegisterAsync(RegisterDto dto);
    Task<Result<TokenResponseDto>> LoginAsync(LoginDto dto);
}
