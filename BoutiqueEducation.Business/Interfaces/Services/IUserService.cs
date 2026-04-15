using BoutiqueEducation.Business.Models;
using BoutiqueEducation.Business.Models.DTOs;

namespace BoutiqueEducation.Business.Interfaces.Services;

public interface IUserService
{
    Task<Result<List<ContactDto>>> GetContactsAsync();
    Task<Result<List<ContactDto>>> GetAllUsersAsync();    // Admin: tüm kullanıcılar
    Task<Result> UpdateUserAsync(Guid userId, UpdateUserDto dto);  // Admin: kullanıcı düzenle
    Task<Result> ChangePasswordAsync(string currentPassword, string newPassword);
    Task<Result<LastMessageDto?>> GetLastMessageAsync();  // Navbar bildirimi
}
