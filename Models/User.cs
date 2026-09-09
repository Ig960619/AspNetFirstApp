using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models;

/// <summary>
/// Модель пользователя, маппится на таблицу users в БД ASPNetDB
/// </summary>
[Table("users")]  // Указываем имя таблицы в БД
public class User
{
    /// <summary>
    /// Уникальный идентификатор (int, auto-increment)
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Имя пользователя (обязательно, до 50 символов)
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Column("Username")]
    public string Username { get; set; } = "";

    /// <summary>
    /// Фамилия (до 50 символов, может быть null)
    /// </summary>
    [MaxLength(50)]
    [Column("UserLastName")]
    public string? UserLastName { get; set; }

    /// <summary>
    /// Отчество (обязательно, до 50 символов)
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Column("UserMiddleName")]
    public string UserMiddleName { get; set; } = "";

    /// <summary>
    /// Имя (до MAX символов, может быть null)
    /// ВАЖНО: В БД колонка называется "User FirstName" с пробелом!
    /// </summary>
    [Column("User FirstName")]  // Имя колонки с пробелом!
    public string? UserFirstName { get; set; }

    /// <summary>
    /// Email (до 50 символов, может быть null)
    /// </summary>
    [MaxLength(50)]
    [Column("Email")]
    public string? Email { get; set; }

    /// <summary>
    /// Телефон (до 50 символов, может быть null)
    /// </summary>
    [MaxLength(50)]
    [Column("Phone")]
    public string? Phone { get; set; }

    /// <summary>
    /// Город (обязательно, до 50 символов)
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Column("City")]
    public string City { get; set; } = "";

    /// <summary>
    /// Пароль (до 50 символов, может быть null)
    /// </summary>
    [MaxLength(50)]
    [Column("Password")]
    public string? Password { get; set; }
}
