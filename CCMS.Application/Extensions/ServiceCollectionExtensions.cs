using CCMS.Application.Interfaces;
using CCMS.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CCMS.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }
}
