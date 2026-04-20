using BoutiqueEducation.Business.Models;
using BoutiqueEducation.Business.Models.DTOs;

namespace BoutiqueEducation.Business.Interfaces.Services;

public interface IDashboardService
{
    Task<Result<DashboardStatsDto>> GetStatsAsync();
}
