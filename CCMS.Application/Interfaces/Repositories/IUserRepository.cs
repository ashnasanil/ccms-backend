using System.Threading.Tasks;
using CCMS.Domain.Entities;

namespace CCMS.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
    }
}
