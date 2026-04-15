namespace BoutiqueEducation.Business.Models.DTOs;

public sealed record DashboardStatsDto(
    int TotalQuestions,
    int PendingQuestions,
    int AnsweredQuestions,
    int TotalTasks,
    int PendingTasks,
    int SubmittedTasks,
    int ApprovedTasks
);
