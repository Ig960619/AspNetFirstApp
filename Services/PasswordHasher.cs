namespace WebApplication1.Services;

using BCrypt.Net;

/// <summary>
/// Реализация хеширования паролей с использованием BCrypt
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    private const int WorkFactor = 12;

    public string Hash(string password)
    {
        return BCrypt.HashPassword(password, WorkFactor);
    }

    public bool Verify(string password, string hash)
    {
        try
        {
            return BCrypt.Verify(password, hash);
        }
        catch
        {
            return false;
        }
    }
}
