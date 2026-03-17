using Huntly.Application.DTOs.Auth;

namespace Huntly.Application.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request); 
        Task<AuthResponse> LoginAsync(LoginRequest request);
    }
}
