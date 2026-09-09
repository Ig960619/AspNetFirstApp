using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Models;

namespace WebApplication1.Models.Auth;

/// <summary>
/// Refresh токен для обновления JWT без повторного логина
/// </summary>
[Table("refresh_tokens")]
public class RefreshToken
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Сам токен (GUID)
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Связанный пользователь
    /// </summary>
    [ForeignKey("User")]
    public int UserId { get; set; }
    public User? User { get; set; }

    /// <summary>
    /// Дата истечения
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Отозван?
    /// </summary>
    public bool IsRevoked { get; set; } = false;

    /// <summary>
    /// IP-адрес, с которого был выдан токен
    /// </summary>
    [MaxLength(45)]
    public string? CreatedByIp { get; set; }

    /// <summary>
    /// IP-адрес, с которого был отозван токен
    /// </summary>
    [MaxLength(45)]
    public string? RevokedByIp { get; set; }

    /// <summary>
    /// Дата отзыва
    /// </summary>
    public DateTime? RevokedAt { get; set; }

    /// <summary>
    /// Токен для замены (при обновлении)
    /// </summary>
    [MaxLength(255)]
    public string? ReplacedByToken { get; set; }

    /// <summary>
    /// Токен активен?
    /// </summary>
    [NotMapped]
    public bool IsActive => !IsRevoked && ExpiresAt > DateTime.UtcNow;
}
