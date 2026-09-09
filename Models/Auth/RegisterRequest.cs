using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.Auth;

/// <summary>
/// DTO для регистрации нового пользователя
/// </summary>
public class RegisterRequest
{
    [Required(ErrorMessage = "Username обязателен")]
    [MaxLength(50, ErrorMessage = "Username не может быть длиннее 50 символов")]
    [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Username может содержать только буквы, цифры и подчёркивание")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email обязателен")]
    [EmailAddress(ErrorMessage = "Некорректный формат email")]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Пароль обязателен")]
    [MinLength(6, ErrorMessage = "Пароль должен быть минимум 6 символов")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Подтверждение пароля обязательно")]
    [Compare("Password", ErrorMessage = "Пароли не совпадают")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Город обязателен")]
    [MaxLength(50)]
    public string City { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? UserFirstName { get; set; }

    [MaxLength(50)]
    public string? UserLastName { get; set; }

    [MaxLength(50)]
    public string? UserMiddleName { get; set; }

    [MaxLength(20)]
    public string? Phone { get; set; }
}
