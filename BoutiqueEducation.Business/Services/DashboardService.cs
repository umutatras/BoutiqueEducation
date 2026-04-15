using BoutiqueEducation.Business.Interfaces.Services;
using BoutiqueEducation.Business.Models.DTOs;
using BoutiqueEducation.Business.Models;
using BoutiqueEducation.DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using EntityTaskStatus = BoutiqueEducation.Entity.Entities.TaskStatus;
using BoutiqueEducation.Entity.Entities;

namespace BoutiqueEducation.Business.Services;

public sealed class DashboardService : IDashboardService
{
    private readonly AppDbContext _context;

    public DashboardService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<DashboardStatsDto>> GetStatsAsync()
    {
        // EF Core DbContext is NOT thread-safe — queries must be sequential, not concurrent
        var totalQuestions   = await _context.Questions.CountAsync();
        var pendingQuestions = await _context.Questions.CountAsync(q => q.Status == QuestionStatus.Pending);
        var answeredQuestions = await _context.Questions.CountAsync(q => q.Status == QuestionStatus.Answered);

        var totalTasks     = await _context.Tasks.CountAsync();
        var pendingTasks   = await _context.Tasks.CountAsync(t => t.Status == EntityTaskStatus.Pending);
        var submittedTasks = await _context.Tasks.CountAsync(t => t.Status == EntityTaskStatus.Submitted);
        var approvedTasks  = await _context.Tasks.CountAsync(t => t.Status == EntityTaskStatus.Approved);

        var stats = new DashboardStatsDto(
            TotalQuestions: totalQuestions,
            PendingQuestions: pendingQuestions,
            AnsweredQuestions: answeredQuestions,
            TotalTasks: totalTasks,
            PendingTasks: pendingTasks,
            SubmittedTasks: submittedTasks,
            ApprovedTasks: approvedTasks
        );

        return Result<DashboardStatsDto>.Success(stats);
    }
}
