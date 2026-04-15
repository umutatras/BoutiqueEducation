using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BoutiqueEducation.API.Controllers;

[Authorize]
[Route("api/files")]
[ApiController]
public sealed class FileController : ControllerBase
{
    private readonly IWebHostEnvironment _env;

    public FileController(IWebHostEnvironment env)
    {
        _env = env;
    }

    /// <summary>
    /// Genel dosya yükleme endpoint'i.
    /// Desteklenen türler: Resim (jpg, png, gif, webp) ve PDF.
    /// </summary>
    [HttpPost("upload")]
    [RequestSizeLimit(10 * 1024 * 1024)] // Max 10 MB
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { Message = "Dosya seçilmedi." });

        // Uzantı kontrolü
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        var allowedExts = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".pdf" };
        if (!allowedExts.Contains(ext))
            return BadRequest(new { Message = "Desteklenmeyen dosya türü. (jpg, png, gif, webp, pdf)" });

        // Klasör: wwwroot/uploads/{images|pdfs}
        var subFolder = ext == ".pdf" ? "pdfs" : "images";
        var uploadsPath = Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"), "uploads", subFolder);
        Directory.CreateDirectory(uploadsPath);

        // Benzersiz dosya adı
        var uniqueName = $"{Guid.NewGuid():N}{ext}";
        var filePath = Path.Combine(uploadsPath, uniqueName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var fileUrl = $"/uploads/{subFolder}/{uniqueName}";
        return Ok(new { Url = fileUrl, FileName = file.FileName, Size = file.Length });
    }
}
