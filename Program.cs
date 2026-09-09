using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Repositories;
using WebApplication1.Services;

var builder = WebApplication.CreateBuilder();

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// DI - Repositories
builder.Services.AddScoped<IUserRepository, SqlUserRepository>();

// DI - Services
builder.Services.AddScoped<IUserService, UserService>();

// MVC Controllers
builder.Services.AddControllers();

var app = builder.Build();

// Тестовый код (удалить после проверки)
using (var scope = app.Services.CreateScope())
{
    var repo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
    var users = repo.GetAll();
    Console.WriteLine($"[DEBUG] Пользователей в БД: {users.Count()}");
}

// Map controllers (маршрутизация через атрибуты)
app.MapControllers();

// Fallback для статической страницы (используем физический путь к файлу)
app.MapFallback(async (context) =>
{
    var filePath = Path.Combine(builder.Environment.ContentRootPath, "html", "index.html");
    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.SendFileAsync(filePath);
});

app.Run();
