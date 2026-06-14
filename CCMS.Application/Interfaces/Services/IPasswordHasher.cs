namespace CCMS.Application.Interfaces.Services
{
    public interface IPasswordHasher
    {
        bool VerifyPassword(string password, string hash);
    }
}
