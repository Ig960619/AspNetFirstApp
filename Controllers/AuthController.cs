using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models.Auth;
using WebApplication1.Services;

namespace WebApplication1.Controllers;

/// <summary>
/// Контроллер авторизации: регистрация, вход, обновление токенов
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Регистрация нового пользователя
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        var ipAddress = GetClientIpAddress();
        var (response, error) = await _authService.RegisterAsync(request, ipAddress);

        if (error != null)
        {
            return BadRequest(new { message = error });
        }

        return Ok(response);
    }

    /// <summary>
    /// Вход в систему
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        var ipAddress = GetClientIpAddress();
        var (response, error) = await _authService.LoginAsync(request, ipAddress);

        if (error != null)
        {
            if (error.Contains("заблокирован"))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = error });
            }
            return BadRequest(new { message = error });
        }

        return Ok(response);
    }

    /// <summary>
    /// Обновление токенов
    /// </summary>
    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var ipAddress = GetClientIpAddress();
        var (response, error) = await _authService.RefreshTokenAsync(
            request.AccessToken,
            request.RefreshToken,
            ipAddress);

        if (error != null)
        {
            return Unauthorized(new { message = error });
        }

        return Ok(response);
    }

    /// <summary>
    /// Выход из системы (отзыв refresh токена)
    /// </summary>
    [HttpPost("revoke")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenRequest request)
    {
        var ipAddress = GetClientIpAddress();
        var result = await _authService.RevokeTokenAsync(request.RefreshToken, ipAddress);

        if (!result)
        {
            return BadRequest(new { message = "Недействительный токен" });
        }

        return NoContent();
    }

    private string? GetClientIpAddress()
    {
        var forwardedFor = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',').FirstOrDefault()?.Trim();
        }

        return HttpContext.Connection.RemoteIpAddress?.ToString();
    }
}

public class RefreshTokenRequest
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}

public class RevokeTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}
