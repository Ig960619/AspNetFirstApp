using System.Security.Claims;
using WebApplication1.Models;
using WebApplication1.Models.Auth;

namespace WebApplication1.Services;

/// <summary>
/// Интерфейс для работы с JWT токенами
/// </summary>
public interface IJwtService
{
    string GenerateAccessToken(User user);
    RefreshToken GenerateRefreshToken(int userId, string? ipAddress);
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    int AccessTokenExpirationMinutes { get; }
    int RefreshTokenExpirationDays { get; }
}
