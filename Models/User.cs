// Models/User.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Models.Auth;

namespace WebApplication1.Models;

[Table("users")]
public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = "";

    [MaxLength(50)]
    public string? UserLastName { get; set; }

    [Required]
    [MaxLength(50)]
    public string UserMiddleName { get; set; } = "";

    [Column("User FirstName")]
    public string? UserFirstName { get; set; }

    [MaxLength(100)]  // Увеличили для email
    public string? Email { get; set; }

    [MaxLength(20)]
    public string? Phone { get; set; }

    [Required]
    [MaxLength(50)]
    public string City { get; set; } = "";

    // === НОВЫЕ ПОЛЯ БЕЗОПАСНОСТИ ===

    /// <summary>
    /// Хешированный пароль (BCrypt)
    /// </summary>
    [MaxLength(255)]
    public string? PasswordHash { get; set; }

    /// <summary>
    /// Email подтверждён?
    /// </summary>
    public bool IsEmailConfirmed { get; set; } = false;

    /// <summary>
    /// Заблокирован до (null = не заблокирован)
    /// </summary>
    public DateTime? LockedUntil { get; set; }

    /// <summary>
    /// Количество неудачных попыток входа
    /// </summary>
    public int FailedLoginAttempts { get; set; } = 0;

    /// <summary>
    /// Дата последнего входа
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    // Навигационное свойство для refresh токенов
    public ICollection<RefreshToken>? RefreshTokens { get; set; }
}
