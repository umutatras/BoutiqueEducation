namespace BoutiqueEducation.Business.Models.DTOs;

public sealed class QuestionCreateDto
{
    public string Content { get; set; } = string.Empty;
    public string Category { get; set; } = "Genel";
    public string? ImageUrl { get; set; }
}
