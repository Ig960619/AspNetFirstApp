using WebApplication1.Models;
using WebApplication1.Repositories;

namespace WebApplication1.Services;

/// <summary>
/// Сервис для работы с пользователями
/// Содержит бизнес-логику и валидацию данных
/// </summary>
public class PersonService : IPersonService
{
    private readonly IPersonRepository _repository;

    public PersonService(IPersonRepository repository)
    {
        _repository = repository;
    }

    public IEnumerable<Person> GetAllUsers()
    {
        return _repository.GetAll();
    }

    public Person? GetUserById(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;

        return _repository.GetById(id);
    }

    public Person? CreateUser(string name, int age)
    {
        // Валидация данных
        if (string.IsNullOrWhiteSpace(name))
            return null;

        if (age < 0 || age > 150)
            return null;

        var person = new Person
        {
            Name = name,
            Age = age
        };

        return _repository.Add(person);
    }

    public Person? UpdateUser(string id, string name, int age)
    {
        // Валидация данных
        if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(name))
            return null;

        if (age < 0 || age > 150)
            return null;

        var existingUser = _repository.GetById(id);
        if (existingUser == null)
            return null;

        existingUser.Name = name;
        existingUser.Age = age;

        _repository.Update(existingUser);
        return existingUser;
    }

    public bool DeleteUser(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return false;

        return _repository.Delete(id);
    }
}
