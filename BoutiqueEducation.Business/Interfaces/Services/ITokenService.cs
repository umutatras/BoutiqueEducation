using BoutiqueEducation.Entity.Entities;

namespace BoutiqueEducation.Business.Interfaces.Services;

public interface ITokenService
{
    Task<string> GenerateJwtTokenAsync(AppUser user);
}
