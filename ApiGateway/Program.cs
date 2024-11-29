using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Добавляем Ocelot конфигурацию
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// Добавляем Ocelot в контейнер сервисов
builder.Services.AddOcelot(builder.Configuration);

// Настройка CORS (если необходимо)
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        policyBuilder => policyBuilder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

// Используем CORS
app.UseCors("CorsPolicy");

// Используем Ocelot
await app.UseOcelot();

app.Run();