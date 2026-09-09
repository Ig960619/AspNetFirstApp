using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers;

/// <summary>
/// REST API контроллер для управления пользователями.
/// Соответствует REST-архитектуре и принципам SOLID.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _service;

    public UserController(IUserService service)
    {
        _service = service;
    }

    /// <summary>
    /// Получить всех пользователей
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<User>> GetAll()
    {
        var users = _service.GetAllUsers();
        return Ok(users);
    }

    /// <summary>
    /// Получить пользователя по ID
    /// </summary>
    [HttpGet("{id:int:min(1)}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<User> GetById(int id)
    {
        var user = _service.GetUserById(id);

        if (user == null)
        {
            return NotFound(new { message = "Пользователь не найден" });
        }

        return Ok(user);
    }

    /// <summary>
    /// Создать нового пользователя (без пароля - используйте /api/auth/register)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<User> Create([FromBody] User userData)
    {
        if (userData == null || string.IsNullOrWhiteSpace(userData.Username) || string.IsNullOrWhiteSpace(userData.City))
        {
            return BadRequest(new { message = "Username и City обязательны" });
        }

        // Создание без пароля (пароль устанавливается через регистрацию)
        var user = _service.CreateUser(
            userData.Username,
            userData.City,
            userData.UserLastName,
            userData.UserMiddleName,
            userData.UserFirstName,
            userData.Email,
            userData.Phone
        );

        if (user == null)
        {
            return BadRequest(new { message = "Некорректные данные" });
        }

        return CreatedAtAction(
            nameof(GetById),
            new { id = user.Id },
            user
        );
    }

    /// <summary>
    /// Обновить данные пользователя
    /// </summary>
    [HttpPut("{id:int:min(1)}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<User> Update(int id, [FromBody] User userData)
    {
        if (userData == null)
        {
            return BadRequest(new { message = "Некорректные данные" });
        }

        var user = _service.UpdateUser(
            id,
            userData.Username,
            userData.City,
            userData.UserLastName,
            userData.UserMiddleName,
            userData.UserFirstName,
            userData.Email,
            userData.Phone
        );

        if (user == null)
        {
            return NotFound(new { message = "Пользователь не найден" });
        }

        return Ok(user);
    }

    /// <summary>
    /// Удалить пользователя
    /// </summary>
    [HttpDelete("{id:int:min(1)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(int id)
    {
        var result = _service.DeleteUser(id);

        if (!result)
        {
            return NotFound(new { message = "Пользователь не найден" });
        }

        return NoContent();
    }
}
