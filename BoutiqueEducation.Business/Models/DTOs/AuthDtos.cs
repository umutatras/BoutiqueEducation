namespace BoutiqueEducation.Business.Models.DTOs;

public sealed class RegisterDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "Student"; // Opsiyonel: Student VEYA Teacher
}

public sealed class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
