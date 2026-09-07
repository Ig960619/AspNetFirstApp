using WebApplication1.Models;

namespace WebApplication1.Services;

/// <summary>
/// Интерфейс сервиса для работы с пользователями
/// Содержит бизнес-логику и валидацию
/// </summary>
public interface IPersonService
{
    /// <summary>
    /// Получить всех пользователей
    /// </summary>
    IEnumerable<Person> GetAllUsers();
    
    /// <summary>
    /// Получить пользователя по идентификатору
    /// </summary>
    Person? GetUserById(string id);
    
    /// <summary>
    /// Создать пользователя
    /// </summary>
    Person? CreateUser(string name, int age);
    
    /// <summary>
    /// Обновить пользователя
    /// </summary>
    Person? UpdateUser(string id, string name, int age);
    
    /// <summary>
    /// Удалить пользователя
    /// </summary>
    bool DeleteUser(string id);
}
