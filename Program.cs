using WebApplication1.Controllers;
using WebApplication1.Repositories;
using WebApplication1.Services;

var builder = WebApplication.CreateBuilder();

// Регистрация зависимостей (Dependency Injection)
// Репозиторий - Singleton, т.к. хранит данные в памяти
builder.Services.AddSingleton<IPersonRepository, PersonRepository>();

// Сервис - Singleton, зависит от репозитория
builder.Services.AddSingleton<IPersonService, PersonService>();

// Контроллер - Singleton, зависит от сервиса
builder.Services.AddSingleton<PersonController>();

var app = builder.Build();

// Получаем контроллер через DI
var personController = app.Services.GetRequiredService<PersonController>();

// Маршрутизация всех запросов через контроллер
app.Run(async (context) =>
{
    await personController.HandleRequestAsync(context);
});

app.Run();
