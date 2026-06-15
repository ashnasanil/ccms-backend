namespace CCMS.Application.Interfaces.Services
{
    public interface ICurrentUserService
    {
        string? Organization { get; }
        string? UserRole { get; }
        string? UserId { get; }
        string? FullName { get; }
    }
}
