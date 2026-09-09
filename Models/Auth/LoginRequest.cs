using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.Auth;

/// <summary>
/// DTO для входа в систему
/// </summary>
public class LoginRequest
{
    [Required(ErrorMessage = "Username обязателен")]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Пароль обязателен")]
    public string Password { get; set; } = string.Empty;
}
