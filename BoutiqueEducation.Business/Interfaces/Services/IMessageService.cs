using BoutiqueEducation.Business.Models;
using BoutiqueEducation.Business.Models.DTOs;

namespace BoutiqueEducation.Business.Interfaces.Services;

public interface IMessageService
{
    Task<Result<MessageResponseDto>> SaveMessageAsync(Guid senderId, Guid receiverId, string content);
    Task<Result<List<MessageResponseDto>>> GetChatHistoryAsync(Guid user1Id, Guid user2Id);
}
