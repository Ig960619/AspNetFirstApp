namespace WebApplication1.Services;

/// <summary>
/// Интерфейс для хеширования и проверки паролей
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Хешировать пароль
    /// </summary>
    string Hash(string password);

    /// <summary>
    /// Проверить пароль против хеша
    /// </summary>
    bool Verify(string password, string hash);
}
