using System.Text.RegularExpressions;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers;

public class UserController
{
    private readonly IUserService _service;
    private readonly string _expressionForId = @"^/api/users/\d+$";

    public UserController(IUserService service)
    {
        _service = service;
    }

    public async Task HandleRequestAsync(HttpContext context)
    {
        var response = context.Response;
        var request = context.Request;
        var path = request.Path;

        if (path == "/api/users" && request.Method == "GET")
        {
            await GetAllUsersAsync(response);
        }
        else if (IsIdPath(path) && request.Method == "GET")
        {
            var id = GetIdFromPath(path);
            await GetUserByIdAsync(id, response);
        }
        else if (path == "/api/users" && request.Method == "POST")
        {
            await CreateUserAsync(request, response);
        }
        else if (IsIdPath(path) && request.Method == "PUT")
        {
            var id = GetIdFromPath(path);
            await UpdateUserAsync(id, request, response);
        }
        else if (IsIdPath(path) && request.Method == "DELETE")
        {
            var id = GetIdFromPath(path);
            await DeleteUserAsync(id, response);
        }
        else
        {
            response.ContentType = "text/html; charset=utf-8";
            await response.SendFileAsync("html/index.html");
        }
    }

    private async Task GetAllUsersAsync(HttpResponse response)
    {
        var users = _service.GetAllUsers();
        await response.WriteAsJsonAsync(users);
    }

    private async Task GetUserByIdAsync(int id, HttpResponse response)
    {
        var user = _service.GetUserById(id);

        if (user != null)
        {
            await response.WriteAsJsonAsync(user);
        }
        else
        {
            response.StatusCode = 404;
            await response.WriteAsJsonAsync(new { message = "Пользователь не найден" });
        }
    }

    private async Task CreateUserAsync(HttpRequest request, HttpResponse response)
    {
        try
        {
            var userData = await request.ReadFromJsonAsync<User>();

            if (userData == null || string.IsNullOrWhiteSpace(userData.Username) || string.IsNullOrWhiteSpace(userData.City))
            {
                response.StatusCode = 400;
                await response.WriteAsJsonAsync(new { message = "Username и City обязательны" });
                return;
            }

            var user = _service.CreateUser(
                userData.Username,
                userData.City,
                userData.UserLastName,
                userData.UserMiddleName,
                userData.UserFirstName,
                userData.Email,
                userData.Phone,
                userData.Password
            );

            if (user == null)
            {
                response.StatusCode = 400;
                await response.WriteAsJsonAsync(new { message = "Некорректные данные" });
                return;
            }

            response.StatusCode = 201;
            response.Headers.Location = $"/api/users/{user.Id}";
            await response.WriteAsJsonAsync(user);
        }
        catch (Exception)
        {
            response.StatusCode = 400;
            await response.WriteAsJsonAsync(new { message = "Некорректные данные" });
        }
    }

    private async Task UpdateUserAsync(int id, HttpRequest request, HttpResponse response)
    {
        try
        {
            var userData = await request.ReadFromJsonAsync<User>();

            if (userData == null)
            {
                response.StatusCode = 400;
                await response.WriteAsJsonAsync(new { message = "Некорректные данные" });
                return;
            }

            var user = _service.UpdateUser(
                id,
                userData.Username,
                userData.City,
                userData.UserLastName,
                userData.UserMiddleName,
                userData.UserFirstName,
                userData.Email,
                userData.Phone,
                userData.Password
            );

            if (user == null)
            {
                response.StatusCode = 404;
                await response.WriteAsJsonAsync(new { message = "Пользователь не найден" });
                return;
            }

            await response.WriteAsJsonAsync(user);
        }
        catch (Exception)
        {
            response.StatusCode = 400;
            await response.WriteAsJsonAsync(new { message = "Некорректные данные" });
        }
    }

    private async Task DeleteUserAsync(int id, HttpResponse response)
    {
        var result = _service.DeleteUser(id);

        if (result)
        {
            response.StatusCode = 204;
        }
        else
        {
            response.StatusCode = 404;
            await response.WriteAsJsonAsync(new { message = "Пользователь не найден" });
        }
    }

    private bool IsIdPath(PathString path)
    {
        return Regex.IsMatch(path, _expressionForId);
    }

    private int GetIdFromPath(PathString path)
    {
        var idStr = path.Value?.Split("/")[3];
        return int.TryParse(idStr, out var id) ? id : 0;
    }
}
