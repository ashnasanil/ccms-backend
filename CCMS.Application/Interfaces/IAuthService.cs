using CCMS.Application.DTOs.Auth;

namespace CCMS.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
    Task LogoutAsync();
}
