namespace BoutiqueEducation.Business.Models.DTOs;

public sealed class TaskCreateDto
{
    public Guid StudentId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
}

public sealed class TaskResponseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string TeacherName { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string? SubmissionImageUrl { get; set; }
    public string? SubmissionFileUrl { get; set; }
    public string? SubmissionNotes { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? DueDate { get; set; }
}
