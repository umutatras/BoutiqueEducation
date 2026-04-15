using BoutiqueEducation.Business.Interfaces.Services;
using BoutiqueEducation.Business.Models;
using BoutiqueEducation.Business.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BoutiqueEducation.API.Controllers;

[Authorize]
[Route("api/questions")]
public sealed class QuestionController : BaseController
{
    private readonly IQuestionService _questionService;

    public QuestionController(IQuestionService questionService)
    {
        _questionService = questionService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateQuestion([FromBody] QuestionCreateDto dto)
    {
        return HandleResult(await _questionService.CreateQuestionAsync(dto));
    }

    [HttpPost("{id}/answer")]
    public async Task<IActionResult> AnswerQuestion(Guid id, [FromBody] AnswerQuestionDto request)
    {
        return HandleResult(await _questionService.AnswerQuestionAsync(id, request.AnswerText, request.AnswerImageUrl), "Soru başarıyla cevaplandı.");
    }

    [HttpGet]
    public async Task<IActionResult> GetAllQuestions([FromQuery] PaginationRequest request)
    {
        return HandleResult(await _questionService.GetAllQuestionsAsync(request));
    }
}
