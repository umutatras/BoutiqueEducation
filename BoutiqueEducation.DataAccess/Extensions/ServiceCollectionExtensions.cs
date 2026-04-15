using BoutiqueEducation.DataAccess.Context;
using BoutiqueEducation.DataAccess.Interfaces;
using BoutiqueEducation.DataAccess.UoW;
using Microsoft.Extensions.DependencyInjection;

namespace BoutiqueEducation.DataAccess.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataAccessServices(this IServiceCollection services)
    {
        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
