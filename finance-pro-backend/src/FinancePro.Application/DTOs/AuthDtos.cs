namespace FinancePro.Application.DTOs;

public record RegisterRequest(string Email, string Password, string FirstName, string LastName, string OrganizationName);
public record LoginRequest(string Email, string Password);
public record AuthResponse(string Token, string RefreshToken, DateTime Expiration, UserDto User);
public record RefreshTokenRequest(string Token, string RefreshToken);
public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
public record UserDto(string Id, string Email, string FirstName, string LastName, string Role, int OrganizationId, string OrganizationName);
