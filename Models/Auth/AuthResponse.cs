namespace WebApplication1.Models.Auth;

/// <summary>
/// Ответ при успешной авторизации
/// </summary>
public class AuthResponse
{
    /// <summary>
    /// JWT токен для авторизации
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Refresh токен для обновления access token
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// Тип токена (Bearer)
    /// </summary>
    public string TokenType { get; set; } = "Bearer";

    /// <summary>
    /// Время жизни access token в секундах
    /// </summary>
    public int ExpiresIn { get; set; }

    /// <summary>
    /// Информация о пользователе
    /// </summary>
    public UserDto User { get; set; } = null!;
}

/// <summary>
/// DTO пользователя (без секретных данных)
/// </summary>
public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? UserFirstName { get; set; }
    public string? UserLastName { get; set; }
    public string City { get; set; } = string.Empty;
}
