using BoutiqueEducation.Business.Models;
using BoutiqueEducation.Business.Models.DTOs;

namespace BoutiqueEducation.Business.Interfaces.Services;

public interface ITaskService
{
    Task<Result<TaskResponseDto>> AssignTaskAsync(TaskCreateDto dto);
    Task<Result> SubmitTaskAsync(Guid taskId, SubmitTaskDto dto);
    Task<Result> ApproveTaskAsync(Guid taskId);
    Task<Result<List<TaskResponseDto>>> GetStudentTasksAsync();
}
