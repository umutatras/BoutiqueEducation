namespace BoutiqueEducation.Business.Models.DTOs;

public sealed class SubmitTaskDto
{
    public string? ImageUrl { get; set; }
    public string? FileUrl { get; set; } // PDF dosya URL'i
    public string? AdditionalNotes { get; set; }
}
