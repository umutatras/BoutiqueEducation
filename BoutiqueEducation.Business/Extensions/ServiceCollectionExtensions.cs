using BoutiqueEducation.Business.Interfaces.Services;
using BoutiqueEducation.Business.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BoutiqueEducation.Business.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBusinessServices(this IServiceCollection services)
    {
        // FluentValidation işlemleri: Sadece geçerli, yüklenebilen tipleri taraması için Assembly üzerinden ReflectionTypeLoadException çözümünü uyguluyoruz.
        // Mevcut Business katmanındaki Validator'leri tarar.
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Servis Kayıtları
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IQuestionService, QuestionService>();
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<IMessageService, MessageService>();

        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}
