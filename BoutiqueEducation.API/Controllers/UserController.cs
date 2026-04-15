using BoutiqueEducation.Business.Interfaces.Services;
using BoutiqueEducation.Business.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BoutiqueEducation.API.Controllers;

[Authorize]
[Route("api/users")]
public sealed class UserController : BaseController
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    // Öğretmen → öğrencilerini, Öğrenci → öğretmenlerini görür
    [HttpGet("contacts")]
    public async Task<IActionResult> GetContacts()
        => HandleResult(await _userService.GetContactsAsync());

    // Navbar bildirimi: giriş yapana gelen son mesaj
    [HttpGet("last-message")]
    public async Task<IActionResult> GetLastMessage()
        => HandleResult(await _userService.GetLastMessageAsync());

    // Kullanıcı kendi şifresini değiştir
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest req)
        => HandleResult(await _userService.ChangePasswordAsync(req.CurrentPassword, req.NewPassword), "Şifre başarıyla değiştirildi.");

    // Admin: tüm kullanıcılar
    [Authorize(Roles = "Admin")]
    [HttpGet("all")]
    public async Task<IActionResult> GetAllUsers()
        => HandleResult(await _userService.GetAllUsersAsync());

    // Admin: kullanıcı güncelle
    [Authorize(Roles = "Admin")]
    [HttpPut("{userId:guid}")]
    public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UpdateUserDto dto)
        => HandleResult(await _userService.UpdateUserAsync(userId, dto), "Kullanıcı güncellendi.");
}

public sealed class ChangePasswordRequest
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
