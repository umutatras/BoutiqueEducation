using BoutiqueEducation.Business.Interfaces.Services;
using BoutiqueEducation.Business.Models;
using BoutiqueEducation.Business.Models.DTOs;
using BoutiqueEducation.DataAccess.Interfaces;
using BoutiqueEducation.Entity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BoutiqueEducation.Business.Services;

public sealed class UserService : IUserService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ICurrentUserService _currentUser;
    private readonly IAppDbContext _context;

    public UserService(UserManager<AppUser> userManager, ICurrentUserService currentUser, IAppDbContext context)
    {
        _userManager = userManager;
        _currentUser = currentUser;
        _context = context;
    }

    // Kişiye göre karşı rol listesi (Teacher → Students, Student → Teachers)
    public async Task<Result<List<ContactDto>>> GetContactsAsync()
    {
        var currentUserId = _currentUser.UserId;
        var currentUser = await _userManager.FindByIdAsync(currentUserId.ToString()!);
        if (currentUser == null) return Result<List<ContactDto>>.Failure("Kullanıcı bulunamadı.");

        var roles = await _userManager.GetRolesAsync(currentUser);
        string targetRole = roles.Contains("Teacher") ? "Student" : "Teacher";

        var usersInRole = await _userManager.GetUsersInRoleAsync(targetRole);
        var contacts = usersInRole
            .Where(u => u.Id != currentUserId)
            .OrderBy(u => u.FullName)
            .Select(u => new ContactDto(u.Id, u.FullName, u.Email ?? "", u.Department))
            .ToList();

        return Result<List<ContactDto>>.Success(contacts);
    }

    // Admin: tüm kullanıcıları listele
    public async Task<Result<List<ContactDto>>> GetAllUsersAsync()
    {
        var users = await _context.Users
            .OrderBy(u => u.FullName)
            .Select(u => new ContactDto(u.Id, u.FullName, u.Email ?? "", u.Department))
            .ToListAsync();

        return Result<List<ContactDto>>.Success(users);
    }

    // Admin: kullanıcı güncelle (isim, bölüm, rol, şifre)
    public async Task<Result> UpdateUserAsync(Guid userId, UpdateUserDto dto)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return Result.Failure("Kullanıcı bulunamadı.");

        user.FullName = dto.FullName;
        user.Department = dto.Department;

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
            return Result.Failure(string.Join(", ", updateResult.Errors.Select(e => e.Description)));

        // Rol değişikliği
        if (!string.IsNullOrWhiteSpace(dto.Role))
        {
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, dto.Role);
        }

        // Şifre değişikliği (admin tarafından)
        if (!string.IsNullOrWhiteSpace(dto.NewPassword))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var pwResult = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);
            if (!pwResult.Succeeded)
                return Result.Failure(string.Join(", ", pwResult.Errors.Select(e => e.Description)));
        }

        return Result.Success();
    }

    // Kullanıcı kendi şifresini değiştir
    public async Task<Result> ChangePasswordAsync(string currentPassword, string newPassword)
    {
        var userId = _currentUser.UserId;
        var user = await _userManager.FindByIdAsync(userId.ToString()!);
        if (user == null) return Result.Failure("Kullanıcı bulunamadı.");

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        if (!result.Succeeded)
            return Result.Failure(string.Join(", ", result.Errors.Select(e => e.Description)));

        return Result.Success();
    }

    // Navbar: giriş yapan kullanıcıya gelen son mesaj
    public async Task<Result<LastMessageDto?>> GetLastMessageAsync()
    {
        var userId = _currentUser.UserId;
        var lastMsg = await _context.Messages
            .Include(m => m.Sender)
            .Where(m => m.ReceiverId == userId)
            .OrderByDescending(m => m.CreatedDate)
            .Select(m => new LastMessageDto
            {
                SenderId = m.SenderId,
                SenderName = m.Sender.FullName,
                Content = m.Content,
                SentAt = m.CreatedDate
            })
            .FirstOrDefaultAsync();

        return Result<LastMessageDto?>.Success(lastMsg);
    }
}
