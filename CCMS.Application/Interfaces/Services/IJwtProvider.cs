using CCMS.Domain.Entities;

namespace CCMS.Application.Interfaces.Services
{
    public interface IJwtProvider
    {
        string GenerateToken(User user);
    }
}
