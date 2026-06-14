using MediatR;
using CCMS.Application.DTOs.Auth;

namespace CCMS.Application.Features.Auth.Commands
{
    public record LoginCommand(string Email, string Password) : IRequest<AuthResponseDto>;
}
