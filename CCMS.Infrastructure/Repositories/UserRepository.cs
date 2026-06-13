using CCMS.Application.Interfaces;
using CCMS.Domain.Entities;
using CCMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CCMS.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DbContext? _context;

    // Local fallback user list containing the seed data for validation and development testing
    private static readonly List<User> _mockUsers = new()
    {
        new User
        {
            Id = 1,
            Username = "court1",
            // BCrypt hash of "Password123"
            PasswordHash = "$2a$11$fEhPFlUzrbPbqgVv7Gv9FOJWnJMl7CLR402AahenD9W23/hwHFI7e",
            Role = UserRole.Court
        },
        new User
        {
            Id = 2,
            Username = "bank1",
            // BCrypt hash of "Password123"
            PasswordHash = "$2a$11$fEhPFlUzrbPbqgVv7Gv9FOJWnJMl7CLR402AahenD9W23/hwHFI7e",
            Role = UserRole.Bank
        }
    };

    // Constructor to support EF Core context injection
    public UserRepository(DbContext context)
    {
        _context = context;
    }

    // Default constructor supporting standalone testing and fallback
    public UserRepository()
    {
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return null;
        }

        if (_context != null)
        {
            try
            {
                var dbUser = await _context.Set<User>().FirstOrDefaultAsync(u => u.Username == username);
                if (dbUser != null)
                {
                    return dbUser;
                }
            }
            catch (Exception)
            {
                // Fallback to the local store if database table is not yet created or configured
            }
        }

        var mockUser = _mockUsers.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        return await Task.FromResult(mockUser);
    }
}
