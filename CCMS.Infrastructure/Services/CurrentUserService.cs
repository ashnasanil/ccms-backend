using System.Security.Claims;
using CCMS.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;

namespace CCMS.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        public string? UserRole => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);
        public string? Organization => _httpContextAccessor.HttpContext?.User?.FindFirstValue("Organization");
    }
}
