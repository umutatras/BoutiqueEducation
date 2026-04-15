namespace BoutiqueEducation.Business.Models.DTOs;

public sealed record ContactDto(Guid Id, string FullName, string Email, string? Department = null);

public sealed class UpdateUserDto
{
    public string FullName { get; set; } = string.Empty;
    public string? Department { get; set; }
    public string? Role { get; set; }
    public string? NewPassword { get; set; }
}

public sealed class LastMessageDto
{
    public Guid SenderId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
}
