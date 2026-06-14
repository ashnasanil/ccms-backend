using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CCMS.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
            services.AddValidatorsFromAssembly(assembly);
            services.AddScoped<CCMS.Application.Interfaces.ICourtService, CCMS.Application.Services.CourtService>();
            
            return services;
        }
    }
}
