using BoutiqueEducation.Business.Models;
using Microsoft.AspNetCore.Mvc;

namespace BoutiqueEducation.API.Controllers;

[ApiController]
public abstract class BaseController : ControllerBase
{
    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (!result.IsSuccess)
            return BadRequest(new { Message = result.ErrorMessage });

        if (result.Data != null)
            return Ok(result.Data);

        return Ok(new { Message = "İşlem başarılı." });
    }

    protected IActionResult HandleResult(Result result, string successMessage = "İşlem başarılı.")
    {
        if (!result.IsSuccess)
            return BadRequest(new { Message = result.ErrorMessage });

        return Ok(new { Message = successMessage });
    }
}
