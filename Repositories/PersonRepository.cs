using WebApplication1.Models;

namespace WebApplication1.Repositories;

/// <summary>
/// Репозиторий для хранения пользователей в памяти
/// В реальном приложении здесь была бы работа с базой данных
/// </summary>
public class PersonRepository : IPersonRepository
{
    private readonly List<Person> _users;

    public PersonRepository()
    {
        // Начальные данные для демонстрации
        _users = new List<Person>
        {
            new() { Id = Guid.NewGuid().ToString(), Name = "Tom", Age = 37 },
            new() { Id = Guid.NewGuid().ToString(), Name = "Bob", Age = 41 },
            new() { Id = Guid.NewGuid().ToString(), Name = "Sam", Age = 24 }
        };
    }

    public IEnumerable<Person> GetAll()
    {
        return _users;
    }

    public Person? GetById(string id)
    {
        return _users.FirstOrDefault(u => u.Id == id);
    }

    public Person Add(Person person)
    {
        person.Id = Guid.NewGuid().ToString();
        _users.Add(person);
        return person;
    }

    public bool Update(Person person)
    {
        var existingUser = _users.FirstOrDefault(u => u.Id == person.Id);
        if (existingUser == null)
            return false;

        existingUser.Name = person.Name;
        existingUser.Age = person.Age;
        return true;
    }

    public bool Delete(string id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user == null)
            return false;

        _users.Remove(user);
        return true;
    }
}
