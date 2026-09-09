using WebApplication1.Models.Auth;

namespace WebApplication1.Services;

/// <summary>
/// Интерфейс сервиса авторизации
/// </summary>
public interface IAuthService
{
    Task<(AuthResponse? Response, string? Error)> RegisterAsync(RegisterRequest request, string? ipAddress);
    Task<(AuthResponse? Response, string? Error)> LoginAsync(LoginRequest request, string? ipAddress);
    Task<(AuthResponse? Response, string? Error)> RefreshTokenAsync(string accessToken, string refreshToken, string? ipAddress);
    Task<bool> RevokeTokenAsync(string refreshToken, string? ipAddress);
}
