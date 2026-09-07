using WebApplication1.Models;

namespace WebApplication1.Repositories;

/// <summary>
/// Интерфейс репозитория для работы с пользователями
/// Абстракция позволяет легко менять источник данных (память, БД, файл)
/// </summary>
public interface IPersonRepository
{
    /// <summary>
    /// Получить всех пользователей
    /// </summary>
    IEnumerable<Person> GetAll();
    
    /// <summary>
    /// Получить пользователя по идентификатору
    /// </summary>
    Person? GetById(string id);
    
    /// <summary>
    /// Добавить пользователя
    /// </summary>
    Person Add(Person person);
    
    /// <summary>
    /// Обновить пользователя
    /// </summary>
    bool Update(Person person);
    
    /// <summary>
    /// Удалить пользователя
    /// </summary>
    bool Delete(string id);
}
