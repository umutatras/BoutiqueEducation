namespace BoutiqueEducation.Business.Models.DTOs;

public sealed class QuestionResponseDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Category { get; set; } = "Genel";
    public string? ImageUrl { get; set; }
    public string Status { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string? TeacherName { get; set; }
    public string? AnswerText { get; set; }
    public string? AnswerImageUrl { get; set; }
    public DateTime CreatedDate { get; set; }
}
