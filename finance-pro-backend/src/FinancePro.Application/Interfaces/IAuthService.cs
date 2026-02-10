using FinancePro.Application.DTOs;

namespace FinancePro.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request);
    Task ChangePasswordAsync(string userId, ChangePasswordRequest request);
    Task<UserDto> GetCurrentUserAsync(string userId);
}
