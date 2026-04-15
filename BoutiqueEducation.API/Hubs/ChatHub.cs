using BoutiqueEducation.Business.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace BoutiqueEducation.API.Hubs;

[Authorize]
public sealed class ChatHub : Hub
{
    private readonly IMessageService _messageService;

    public ChatHub(IMessageService messageService)
    {
        _messageService = messageService;
    }

    public async Task SendMessage(Guid receiverId, string content)
    {
        var userIdStr = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdStr, out Guid senderId)) return;

        var result = await _messageService.SaveMessageAsync(senderId, receiverId, content);
        if (!result.IsSuccess) return;

        var message = result.Data!;

        // 1) Alıcıya gönder (sadece alıcının grubuna, Caller hariç)
        await Clients.GroupExcept($"User_{receiverId}", Context.ConnectionId)
            .SendAsync("ReceiveMessage", message);

        // 2) Gönderene tek seferlik onay (MessageSent = kendi ekranında görünsün)
        //    ReceiveMessage GÖNDERİLMEZ — duplicate önlemi
        await Clients.Caller.SendAsync("MessageSent", message);
    }

    public override async Task OnConnectedAsync()
    {
        var userIdStr = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userIdStr))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userIdStr}");
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userIdStr = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userIdStr))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userIdStr}");
        }
        await base.OnDisconnectedAsync(exception);
    }
}
