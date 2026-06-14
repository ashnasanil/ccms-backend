using System.Threading.Tasks;
using CCMS.Application.Interfaces.Repositories;
using CCMS.Domain.Entities;
using CCMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CCMS.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
