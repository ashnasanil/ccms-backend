using CCMS.Application.Interfaces;
using CCMS.Infrastructure.Repositories;
using CCMS.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CCMS.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IJwtService, JwtService>();

        services.AddScoped<CCMS.Application.Interfaces.Repositories.IUserRepository, UserRepository>();

        return services;
    }
}
