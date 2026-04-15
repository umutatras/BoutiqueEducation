using BoutiqueEducation.Business.Models;
using BoutiqueEducation.Business.Models.DTOs;

namespace BoutiqueEducation.Business.Interfaces.Services;

public interface IQuestionService
{
    Task<Result<QuestionResponseDto>> CreateQuestionAsync(QuestionCreateDto dto);
    Task<Result> AnswerQuestionAsync(Guid questionId, string? answerText, string? answerImageUrl);
    /// <summary>
    /// Öğretmenler kendi bölümlerine göre, öğrenciler tüm soruları görür.
    /// </summary>
    Task<Result<PagedResponse<QuestionResponseDto>>> GetAllQuestionsAsync(PaginationRequest request);
}
