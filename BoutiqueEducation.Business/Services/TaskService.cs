using BoutiqueEducation.Business.Interfaces.Services;
using BoutiqueEducation.Business.Models;
using BoutiqueEducation.Business.Models.DTOs;
using BoutiqueEducation.DataAccess.Interfaces;
using BoutiqueEducation.Entity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskStatus = BoutiqueEducation.Entity.Entities.TaskStatus;

namespace BoutiqueEducation.Business.Services;

public sealed class TaskService : ITaskService
{
    private readonly IAppDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly UserManager<AppUser> _userManager;

    public TaskService(
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

    public async Task<Result<TaskResponseDto>> AssignTaskAsync(TaskCreateDto dto)
    {
        var teacherId = _currentUserService.UserId;

        var student = await _context.Users.FirstOrDefaultAsync(u => u.Id == dto.StudentId);
        if (student == null)
            return Result<TaskResponseDto>.Failure("Öğrenci bulunamadı.");

        var taskItem = new TaskItem
        {
            TeacherId = teacherId,
            StudentId = dto.StudentId,
            Title = dto.Title,
            Description = dto.Description,
            DueDate = dto.DueDate,
            Status = TaskStatus.Pending
        };

        await _context.Tasks.AddAsync(taskItem);
        await _unitOfWork.SaveChangesAsync();

        var teacher = await _context.Users.FirstOrDefaultAsync(u => u.Id == teacherId);

        return Result<TaskResponseDto>.Success(new TaskResponseDto
        {
            Id = taskItem.Id,
            Title = taskItem.Title,
            Description = taskItem.Description,
            Status = taskItem.Status.ToString(),
            TeacherName = teacher?.FullName ?? "",
            StudentName = student.FullName,
            CreatedDate = taskItem.CreatedDate,
            DueDate = taskItem.DueDate
        });
    }

    public async Task<Result> SubmitTaskAsync(Guid taskId, SubmitTaskDto dto)
    {
        var studentId = _currentUserService.UserId;
        var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == taskId && t.StudentId == studentId);

        if (task == null)
            return Result.Failure("Görev bulunamadı veya size ait değil.");

        if (task.Status != TaskStatus.Pending)
            return Result.Failure("Bu görev için zaten çıktı yüklenmiş.");

        task.SubmissionImageUrl = dto.ImageUrl;
        task.SubmissionFileUrl = dto.FileUrl;
        task.SubmissionNotes = dto.AdditionalNotes;
        task.Status = TaskStatus.Submitted;
        task.SubmittedAt = DateTime.Now;

        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result> ApproveTaskAsync(Guid taskId)
    {
        var teacherId = _currentUserService.UserId;
        var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == taskId && t.TeacherId == teacherId);

        if (task == null)
            return Result.Failure("Görev bulunamadı veya yetkiniz yok.");

        if (task.Status != TaskStatus.Submitted)
            return Result.Failure("Öğrenci henüz görevi teslim etmemiş.");

        task.Status = TaskStatus.Approved;
        task.ApprovedAt = DateTime.Now;
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }

    /// <summary>
    /// Role-based: Öğrenci → kendi ödevlerini, Öğretmen → kendi atadığı ödevleri görür
    /// </summary>
    public async Task<Result<List<TaskResponseDto>>> GetStudentTasksAsync()
    {
        var userId = _currentUserService.UserId;
        var user = await _userManager.FindByIdAsync(userId.ToString()!);
        var roles = user != null ? await _userManager.GetRolesAsync(user) : new List<string>();

        var query = _context.Tasks
            .Include(t => t.Teacher)
            .Include(t => t.Student)
            .AsQueryable();

        if (roles.Contains("Teacher") || roles.Contains("Admin"))
        {
            // Öğretmen kendi atadığı ödevleri görür
            query = query.Where(t => t.TeacherId == userId);
        }
        else
        {
            // Öğrenci kendi ödevlerini görür
            query = query.Where(t => t.StudentId == userId);
        }

        var tasks = await query
            .OrderByDescending(t => t.CreatedDate)
            .Select(t => new TaskResponseDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status.ToString(),
                TeacherName = t.Teacher.FullName,
                StudentName = t.Student.FullName,
                SubmissionImageUrl = t.SubmissionImageUrl,
                SubmissionFileUrl = t.SubmissionFileUrl,
                SubmissionNotes = t.SubmissionNotes,
                CreatedDate = t.CreatedDate,
                DueDate = t.DueDate
            })
            .ToListAsync();

        return Result<List<TaskResponseDto>>.Success(tasks);
    }
}
