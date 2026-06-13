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

        // Safely register IUserRepository. If DbContext is registered, it will inject it.
        // Otherwise, it falls back to the mock implementation using the default constructor.
        services.AddScoped<IUserRepository>(provider =>
        {
            var dbContext = provider.GetService<DbContext>();
            return dbContext != null ? new UserRepository(dbContext) : new UserRepository();
        });

        return services;
    }
}
