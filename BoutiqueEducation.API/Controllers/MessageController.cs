using BoutiqueEducation.Business.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BoutiqueEducation.API.Controllers;

[Authorize]
[Route("api/messages")]
public sealed class MessageController : BaseController
{
    private readonly IMessageService _messageService;
    private readonly ICurrentUserService _currentUserService;

    public MessageController(IMessageService messageService, ICurrentUserService currentUserService)
    {
        _messageService = messageService;
        _currentUserService = currentUserService;
    }

    [HttpGet("history/{contactId:guid}")]
    public async Task<IActionResult> GetHistory(Guid contactId)
    {
        var currentUserId = _currentUserService.UserId;
        var result = await _messageService.GetChatHistoryAsync(currentUserId, contactId);
        return HandleResult(result);
    }
}
