using CCMS.Application.DTOs.Auth;
using CCMS.Application.Exceptions;
using CCMS.Application.Interfaces;
using CCMS.Domain.Entities;

namespace CCMS.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;

    public AuthService(IUserRepository userRepository, IJwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            throw new UnauthorizedException("Username and password are required.");
        }

        Console.WriteLine($"[AuthService] Attempting login for username: '{request.Username}'");
        var user = await _userRepository.GetByUsernameAsync(request.Username);
        if (user == null)
        {
            Console.WriteLine($"[AuthService] User '{request.Username}' not found in repository.");
            throw new UnauthorizedException("Invalid username or password.");
        }

        Console.WriteLine($"[AuthService] User '{user.Email}' found. Role: {user.Role}. Stored Hash: {user.PasswordHash}");

        // Verify password using BCrypt
        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        Console.WriteLine($"[AuthService] Password verification result: {isPasswordValid}");
        if (!isPasswordValid)
        {
            throw new UnauthorizedException("Invalid username or password.");
        }

        string token = _jwtService.GenerateToken(user);

        return new LoginResponseDto
        {
            Token = token,
            Role = user.Role.ToString()
        };
    }

    public Task LogoutAsync()
    {
        // No server-side session handling as per requirements.
        return Task.CompletedTask;
    }
}
