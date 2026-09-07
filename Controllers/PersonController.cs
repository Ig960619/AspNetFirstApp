using System.Text.RegularExpressions;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers;

/// <summary>
/// Контроллер для обработки HTTP запросов к API пользователей
/// Отвечает за маршрутизацию, парсинг запросов и формирование ответов
/// </summary>
public class PersonController
{
    private readonly IPersonService _service;
    
    // Регулярное выражение для проверки GUID в URL
    private readonly string _expressionForGuid = @"^/api/users/\w{8}-\w{4}-\w{4}-\w{4}-\w{12}$";

    public PersonController(IPersonService service)
    {
        _service = service;
    }

    /// <summary>
    /// Обрабатывает входящий HTTP запрос
    /// </summary>
    public async Task HandleRequestAsync(HttpContext context)
    {
        var response = context.Response;
        var request = context.Request;
        var path = request.Path;

        // GET /api/users - получить всех пользователей
        if (path == "/api/users" && request.Method == "GET")
        {
            await GetAllUsersAsync(response);
        }
        // GET /api/users/{id} - получить пользователя по id
        else if (IsGuidPath(path) && request.Method == "GET")
        {
            var id = GetIdFromPath(path);
            await GetUserByIdAsync(id, response);
        }
        // POST /api/users - создать пользователя
        else if (path == "/api/users" && request.Method == "POST")
        {
            await CreateUserAsync(request, response);
        }
        // PUT /api/users/{id} - обновить пользователя
        else if (IsGuidPath(path) && request.Method == "PUT")
        {
            var id = GetIdFromPath(path);
            await UpdateUserAsync(id, request, response);
        }
        // DELETE /api/users/{id} - удалить пользователя
        else if (IsGuidPath(path) && request.Method == "DELETE")
        {
            var id = GetIdFromPath(path);
            await DeleteUserAsync(id, response);
        }
        // Если маршрут не найден - возвращаем главную страницу
        else
        {
            response.ContentType = "text/html; charset=utf-8";
            await response.SendFileAsync("html/index.html");
        }
    }

    /// <summary>
    /// Получить всех пользователей
    /// </summary>
    private async Task GetAllUsersAsync(HttpResponse response)
    {
        var users = _service.GetAllUsers();
        await response.WriteAsJsonAsync(users);
    }

    /// <summary>
    /// Получить пользователя по идентификатору
    /// </summary>
    private async Task GetUserByIdAsync(string? id, HttpResponse response)
    {
        var user = _service.GetUserById(id!);
        
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

    /// <summary>
    /// Создать нового пользователя
    /// </summary>
    private async Task CreateUserAsync(HttpRequest request, HttpResponse response)
    {
        try
        {
            var userData = await request.ReadFromJsonAsync<Person>();
            
            if (userData == null)
            {
                response.StatusCode = 400;
                await response.WriteAsJsonAsync(new { message = "Некорректные данные" });
                return;
            }

            var user = _service.CreateUser(userData.Name, userData.Age);
            
            if (user == null)
            {
                response.StatusCode = 400;
                await response.WriteAsJsonAsync(new { message = "Некорректные данные" });
                return;
            }

            // Возвращаем 201 Created с заголовком Location
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

    /// <summary>
    /// Обновить пользователя
    /// </summary>
    private async Task UpdateUserAsync(string? id, HttpRequest request, HttpResponse response)
    {
        try
        {
            var userData = await request.ReadFromJsonAsync<Person>();
            
            if (userData == null)
            {
                response.StatusCode = 400;
                await response.WriteAsJsonAsync(new { message = "Некорректные данные" });
                return;
            }

            var user = _service.UpdateUser(id!, userData.Name, userData.Age);
            
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

    /// <summary>
    /// Удалить пользователя
    /// </summary>
    private async Task DeleteUserAsync(string? id, HttpResponse response)
    {
        var result = _service.DeleteUser(id!);
        
        if (result)
        {
            // Возвращаем 204 No Content при успешном удалении
            response.StatusCode = 204;
        }
        else
        {
            response.StatusCode = 404;
            await response.WriteAsJsonAsync(new { message = "Пользователь не найден" });
        }
    }

    /// <summary>
    /// Проверяет, соответствует ли путь формату /api/users/{guid}
    /// </summary>
    private bool IsGuidPath(PathString path)
    {
        return Regex.IsMatch(path, _expressionForGuid);
    }

    /// <summary>
    /// Извлекает ID из пути URL
    /// </summary>
    private string? GetIdFromPath(PathString path)
    {
        return path.Value?.Split("/")[3];
    }
}
