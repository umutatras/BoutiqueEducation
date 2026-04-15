using BoutiqueEducation.Business.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace BoutiqueEducation.Business.Services;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId
    {
        get
        {
            var userIdStr = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdStr, out Guid userId) ? userId : Guid.Empty;
        }
    }
}
