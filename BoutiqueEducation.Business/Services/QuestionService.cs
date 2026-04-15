using BoutiqueEducation.Business.Extensions;
using BoutiqueEducation.Business.Interfaces.Services;
using BoutiqueEducation.Business.Models;
using BoutiqueEducation.Business.Models.DTOs;
using BoutiqueEducation.DataAccess.Interfaces;
using BoutiqueEducation.Entity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BoutiqueEducation.Business.Services;

public sealed class QuestionService : IQuestionService
{
    private readonly IAppDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly UserManager<AppUser> _userManager;

    public QuestionService(
        IAppDbContext context,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        UserManager<AppUser> userManager)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _userManager = userManager;
    }

    public async Task<Result<QuestionResponseDto>> CreateQuestionAsync(QuestionCreateDto dto)
    {
        var studentId = _currentUserService.UserId;
        var studentExists = await _context.Users.AnyAsync(u => u.Id == studentId);
        if (!studentExists)
            return Result<QuestionResponseDto>.Failure("Öğrenci bulunamadı.");

        var question = new Question
        {
            StudentId = studentId,
            Content = dto.Content,
            Category = string.IsNullOrWhiteSpace(dto.Category) ? "Genel" : dto.Category,
            ImageUrl = dto.ImageUrl,
            Status = QuestionStatus.Pending
        };

        await _context.Questions.AddAsync(question);
        await _unitOfWork.SaveChangesAsync();

        return Result<QuestionResponseDto>.Success(new QuestionResponseDto
        {
            Id = question.Id,
            Content = question.Content,
            Category = question.Category,
            ImageUrl = question.ImageUrl,
            Status = question.Status.ToString(),
            StudentName = ""
        });
    }

    public async Task<Result> AnswerQuestionAsync(Guid questionId, string? answerText, string? answerImageUrl)
    {
        var teacherId = _currentUserService.UserId;
        var question = await _context.Questions.FirstOrDefaultAsync(q => q.Id == questionId);

        if (question == null) return Result.Failure("Soru bulunamadı.");
        if (question.Status == QuestionStatus.Answered) return Result.Failure("Bu soru zaten cevaplanmış.");
        if (string.IsNullOrWhiteSpace(answerText) && string.IsNullOrWhiteSpace(answerImageUrl))
            return Result.Failure("Cevap metni veya resmi göndermelisiniz.");

        question.TeacherId = teacherId;
        question.AnswerText = answerText;
        question.AnswerImageUrl = answerImageUrl;
        question.Status = QuestionStatus.Answered;

        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result<PagedResponse<QuestionResponseDto>>> GetAllQuestionsAsync(PaginationRequest request)
    {
        var userId = _currentUserService.UserId;
        var user = await _userManager.FindByIdAsync(userId.ToString()!);
        var roles = user != null ? await _userManager.GetRolesAsync(user) : [];

        bool isTeacher = roles.Contains("Teacher");

        // Öğretmen → sadece kendi bölümüne (Department) ait kategorideki soruları görsün
        // Öğrenci/Admin → tüm soruları görsün
        var query = _context.Questions
            .Include(q => q.Student)
            .Include(q => q.Teacher)
            .AsQueryable();

        if (isTeacher && user?.Department != null)
        {
            query = query.Where(q => q.Category == user.Department);
        }

        var pagedData = await query
            .OrderByDescending(q => q.CreatedDate)
            .Select(q => new QuestionResponseDto
            {
                Id = q.Id,
                Content = q.Content,
                Category = q.Category,
                ImageUrl = q.ImageUrl,
                Status = q.Status.ToString(),
                StudentName = q.Student.FullName,
                TeacherName = q.Teacher != null ? q.Teacher.FullName : null,
                AnswerText = q.AnswerText,
                AnswerImageUrl = q.AnswerImageUrl,
                CreatedDate = q.CreatedDate
            })
            .ToPagedResponseAsync(request.PageNumber, request.PageSize);

        return Result<PagedResponse<QuestionResponseDto>>.Success(pagedData);
    }
}
