using WebApplication1.Models;

namespace WebApplication1.Services;

public interface IUserService
{
    IEnumerable<User> GetAllUsers();
    User? GetUserById(int id);
    User? CreateUser(string username, string city, string? lastName, string? middleName, string? firstName, string? email, string? phone);
    User? UpdateUser(int id, string username, string city, string? lastName, string? middleName, string? firstName, string? email, string? phone);
    bool DeleteUser(int id);
}
