using System;
using System.Linq;
using CCMS.Application.Interfaces.Repositories;
using CCMS.Application.Interfaces.Services;
using CCMS.Domain.Entities;
using CCMS.Domain.Enums;
using CCMS.Infrastructure.Persistence;
using CCMS.Infrastructure.Repositories;
using CCMS.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CCMS.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            
            services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.Parse("8.0.32-mysql")));

            services.AddHttpContextAccessor();

            services.AddScoped<ICaseRepository, CaseRepository>();
            services.AddScoped<ICaseResponseRepository, CaseResponseRepository>();
            services.AddScoped<IBatchJobRepository, BatchJobRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IMaskingService, MaskingService>();
            services.AddScoped<IJwtProvider, JwtTokenService>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ILocalFileStorageService, LocalFileStorageService>();
            services.AddScoped<IBankAccountVerificationService, BankAccountVerificationService>();

            return services;
        }

        public static void SeedDatabase(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (context.Database.IsRelational())
            {
                context.Database.Migrate();
            }
            else
            {
                context.Database.EnsureCreated();
            }

            if (!context.Users.Any())
            {
                var passwordHash = BCrypt.Net.BCrypt.HashPassword("Password@123");
                context.Users.AddRange(
                    new User { Email = "court@ccms.local", PasswordHash = passwordHash, FullName = "Court Officer", Role = UserRole.CourtOfficer, Organization = "Court" },
                    new User { Email = "bank@ccms.local", PasswordHash = passwordHash, FullName = "Bank Officer", Role = UserRole.BankOfficer, Organization = "Test Bank" }
                );
                context.SaveChanges();
            }
            else
            {
                var bankUser = context.Users.FirstOrDefault(u => u.Email == "bank@ccms.local");
                if (bankUser != null && bankUser.Organization == "Bank")
                {
                    bankUser.Organization = "Test Bank";
                    context.SaveChanges();
                }
            }

            if (!context.BankCustomers.Any())
            {
                context.BankCustomers.AddRange(
                    new BankCustomer { AccountNumber = "1234567890", AadhaarNumber = "123456789012", PanNumber = "ABCDE1234F", Balance = 50000.00m, AccountStatus = "Active", BankName = "Test Bank" },
                    new BankCustomer { AccountNumber = "0987654321", AadhaarNumber = "210987654321", PanNumber = "FGHIJ5678K", Balance = 1500.50m, AccountStatus = "Dormant", BankName = "Test Bank" }
                );
                context.SaveChanges();
            }
            if (context.BankCustomers.Count() <= 2)
            {
                context.BankCustomers.AddRange(
                    new BankCustomer { AccountNumber = "1122334455", AadhaarNumber = "111122223333", PanNumber = "ABCDE1111F", Balance = 120000.00m, AccountStatus = "Active", BankName = "Test Bank" },
                    new BankCustomer { AccountNumber = "9988776655", AadhaarNumber = "999988887777", PanNumber = "ZYXWV9999U", Balance = 10.50m, AccountStatus = "Frozen", BankName = "Test Bank" }
                );
                context.SaveChanges();
            }

            // Cases are intentionally NOT seeded to allow starting from scratch

            context.SaveChanges();
        }
    }
}
