namespace BoutiqueEducation.Business.Models.DTOs;

public sealed record ContactDto(Guid Id, string FullName, string Email, string? Department = null);

/// <summary>Admin kullanıcı listesi (rol ve onay bilgisi dahil)</summary>
public sealed class AdminUserDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Department { get; set; }
    public string Role { get; set; } = string.Empty;
    public bool IsApproved { get; set; }
    public DateTime CreatedDate { get; set; }
}

public sealed class UpdateUserDto
{
    public string FullName { get; set; } = string.Empty;
    public string? Department { get; set; }
    public string? Role { get; set; }
    public string? NewPassword { get; set; }
    public bool? IsApproved { get; set; }
}

public sealed class LastMessageDto
{
    public Guid SenderId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
}
