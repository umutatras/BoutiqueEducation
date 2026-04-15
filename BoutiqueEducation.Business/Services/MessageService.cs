using BoutiqueEducation.Business.Interfaces.Services;
using BoutiqueEducation.Business.Models;
using BoutiqueEducation.Business.Models.DTOs;
using BoutiqueEducation.DataAccess.Interfaces;
using BoutiqueEducation.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace BoutiqueEducation.Business.Services;

public sealed class MessageService : IMessageService
{
    private readonly IAppDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public MessageService(IAppDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<MessageResponseDto>> SaveMessageAsync(Guid senderId, Guid receiverId, string content)
    {
        var message = new Message
        {
            SenderId = senderId,
            ReceiverId = receiverId,
            Content = content
        };

        await _context.Messages.AddAsync(message);
        await _unitOfWork.SaveChangesAsync();

        var senderName = await _context.Users
            .Where(u => u.Id == senderId)
            .Select(u => u.FullName)
            .FirstOrDefaultAsync() ?? "Bilinmiyor";

        return Result<MessageResponseDto>.Success(new MessageResponseDto
        {
            Id = message.Id,
            Content = message.Content,
            SentAt = message.CreatedDate,
            SenderId = message.SenderId,
            SenderName = senderName,
            ReceiverId = message.ReceiverId
        });
    }

    public async Task<Result<List<MessageResponseDto>>> GetChatHistoryAsync(Guid user1Id, Guid user2Id)
    {
        var messages = await _context.Messages
            .Include(m => m.Sender)
            .Where(m => (m.SenderId == user1Id && m.ReceiverId == user2Id) ||
                        (m.SenderId == user2Id && m.ReceiverId == user1Id))
            .OrderBy(m => m.CreatedDate)
            .Select(m => new MessageResponseDto
            {
                Id = m.Id,
                Content = m.Content,
                SentAt = m.CreatedDate,
                SenderId = m.SenderId,
                SenderName = m.Sender.FullName,
                ReceiverId = m.ReceiverId
            })
            .ToListAsync();

        return Result<List<MessageResponseDto>>.Success(messages);
    }
}
