using CCMS.Domain.Entities;

namespace CCMS.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
}
