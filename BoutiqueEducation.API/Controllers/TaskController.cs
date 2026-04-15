using BoutiqueEducation.Business.Interfaces.Services;
using BoutiqueEducation.Business.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BoutiqueEducation.API.Controllers;

[Authorize]
[Route("api/tasks")]
public sealed class TaskController : BaseController
{
    private readonly ITaskService _taskService;

    public TaskController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpPost]
    public async Task<IActionResult> AssignTask([FromBody] TaskCreateDto dto)
    {
        return HandleResult(await _taskService.AssignTaskAsync(dto));
    }

    [HttpPost("{id}/submit")]
    public async Task<IActionResult> SubmitTask(Guid id, [FromBody] SubmitTaskDto request)
    {
        return HandleResult(await _taskService.SubmitTaskAsync(id, request), "Ödev başarıyla teslim edildi.");
    }

    [HttpPost("{id}/approve")]
    public async Task<IActionResult> ApproveTask(Guid id)
    {
        return HandleResult(await _taskService.ApproveTaskAsync(id), "Ödev onaylandı.");
    }

    [HttpGet("student")]
    public async Task<IActionResult> GetStudentTasks()
    {
        return HandleResult(await _taskService.GetStudentTasksAsync());
    }
}
