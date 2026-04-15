using BoutiqueEducation.Business.Models.DTOs;
using BoutiqueEducation.Business.Models;
using System.Threading.Tasks;

namespace BoutiqueEducation.Business.Interfaces.Services;

public interface IDashboardService
{
    Task<Result<DashboardStatsDto>> GetStatsAsync();
}
