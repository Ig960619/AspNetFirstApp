using WebApplication1.Models;
using WebApplication1.Repositories;

namespace WebApplication1.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }

    public IEnumerable<User> GetAllUsers()
    {
        return _repository.GetAll();
    }

    public User? GetUserById(int id)
    {
        return _repository.GetById(id);
    }

    public User? CreateUser(string username, string city, string? lastName, string? middleName, string? firstName, string? email, string? phone, string? password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(city))
            return null;

        var user = new User
        {
            Username = username,
            City = city,
            UserLastName = lastName,
            UserMiddleName = middleName ?? "",
            UserFirstName = firstName,
            Email = email,
            Phone = phone,
            Password = password
        };

        return _repository.Add(user);
    }

    public User? UpdateUser(int id, string username, string city, string? lastName, string? middleName, string? firstName, string? email, string? phone, string? password)
    {
        var existing = _repository.GetById(id);
        if (existing == null) return null;

        existing.Username = username;
        existing.City = city;
        existing.UserLastName = lastName;
        existing.UserMiddleName = middleName ?? "";
        existing.UserFirstName = firstName;
        existing.Email = email;
        existing.Phone = phone;
        existing.Password = password;

        _repository.Update(existing);
        return existing;
    }

    public bool DeleteUser(int id)
    {
        return _repository.Delete(id);
    }
}
