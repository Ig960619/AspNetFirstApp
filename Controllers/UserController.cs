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
    /// <returns>Список всех пользователей</returns>
    /// <response code="200">Возвращает список пользователей</response>
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
    /// <param name="id">ID пользователя</param>
    /// <returns>Пользователь с указанным ID</returns>
    /// <response code="200">Возвращает пользователя</response>
    /// <response code="404">Пользователь не найден</response>
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
    /// Создать нового пользователя
    /// </summary>
    /// <param name="userData">Данные пользователя</param>
    /// <returns>Созданный пользователь</returns>
    /// <response code="201">Пользователь успешно создан</response>
    /// <response code="400">Некорректные данные</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<User> Create([FromBody] User userData)
    {
        if (userData == null || string.IsNullOrWhiteSpace(userData.Username) || string.IsNullOrWhiteSpace(userData.City))
        {
            return BadRequest(new { message = "Username и City обязательны" });
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
    /// <param name="id">ID пользователя</param>
    /// <param name="userData">Обновленные данные</param>
    /// <returns>Обновленный пользователь</returns>
    /// <response code="200">Пользователь успешно обновлен</response>
    /// <response code="400">Некорректные данные</response>
    /// <response code="404">Пользователь не найден</response>
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
            userData.Phone,
            userData.Password
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
    /// <param name="id">ID пользователя</param>
    /// <response code="204">Пользователь успешно удален</response>
    /// <response code="404">Пользователь не найден</response>
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
