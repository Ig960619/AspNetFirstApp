using Microsoft.EntityFrameworkCore;
using WebApplication1.Controllers;
using WebApplication1.Data;
using WebApplication1.Repositories;
using WebApplication1.Services;

var builder = WebApplication.CreateBuilder();

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// DI
builder.Services.AddScoped<IUserRepository, SqlUserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<UserController>();

var app = builder.Build();

// Тестовый код (удалить после проверки)
using (var scope = app.Services.CreateScope())
{
    var repo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
    var users = repo.GetAll();
    Console.WriteLine($"[DEBUG] Пользователей в БД: {users.Count()}");
}

app.Run(async (context) =>
{
    // Получаем контроллер через DI внутри запроса
    var userController = context.RequestServices.GetRequiredService<UserController>();
    await userController.HandleRequestAsync(context);
});

app.Run();
