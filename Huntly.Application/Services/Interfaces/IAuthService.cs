using Huntly.Application.DTOs.Auth;

namespace Huntly.Application.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request); Task<>
        Task<AuthResponse> LoginAsync(LoginRequest request);
    }
}
