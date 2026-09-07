namespace WebApplication1.Models;

/// <summary>
/// Модель пользователя
/// </summary>
public class Person
{
    /// <summary>
    /// Уникальный идентификатор пользователя
    /// </summary>
    public string Id { get; set; } = "";
    
    /// <summary>
    /// Имя пользователя
    /// </summary>
    public string Name { get; set; } = "";
    
    /// <summary>
    /// Возраст пользователя
    /// </summary>
    public int Age { get; set; }
}
