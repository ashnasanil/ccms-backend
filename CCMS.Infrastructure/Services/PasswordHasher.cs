using CCMS.Application.Interfaces.Services;

namespace CCMS.Infrastructure.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        public bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
