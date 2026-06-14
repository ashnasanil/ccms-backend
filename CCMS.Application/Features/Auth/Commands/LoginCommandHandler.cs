using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using CCMS.Application.DTOs.Auth;
using CCMS.Application.Interfaces.Repositories;
using CCMS.Application.Interfaces.Services;

namespace CCMS.Application.Features.Auth.Commands
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtProvider _jwtProvider;

        public LoginCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtProvider jwtProvider)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtProvider = jwtProvider;
        }

        public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            var token = _jwtProvider.GenerateToken(user);
            return new AuthResponseDto
            {
                Token = token,
                Role = user.Role.ToString(),
                Organization = user.Organization
            };
        }
    }
}
