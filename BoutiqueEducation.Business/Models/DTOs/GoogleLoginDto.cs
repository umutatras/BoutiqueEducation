namespace BoutiqueEducation.Business.Models.DTOs;

public sealed class GoogleLoginDto
{
    public string IdToken { get; set; } = string.Empty;
}

public sealed class TokenResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public System.DateTime Expiration { get; set; }
}
