using AuthService.API.Middlewares;
using AuthService.Application.MappingProfiles;
using AuthService.Application.Services;
using AuthService.Domain.Abstractions;
using AuthService.Infrastructure;
using AuthService.Infrastructure.Kafka;
using AuthService.Infrastructure.Repositories;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();

// ��������� Kestrel ��� ������������� HTTP �� ����� 8081
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8082, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging(); // ��������� �����������
/*builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));*/
builder.Services.AddDbContext(builder.Configuration);

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IJwtProvider, JwtProvider>();

//Producer configs
builder.Services.AddScoped<IProducer<Null, String>>(provider =>
{
    var config = new ProducerConfig()
    {
        BootstrapServers = builder.Configuration["Kafka:BootstrapServers"],
        /*Acks = Acks.Leader,
        EnableIdempotence = true,*/
    };

    return new ProducerBuilder<Null, string>(config).Build();
});
builder.Services.AddScoped<IKafkaProducer, KafkaProducer>();
builder.Services.AddScoped<IKafkaProducerService, KafkaProducerService>();
builder.Services.AddHostedService<KafkaConsumerService>();

//Настройка Kafka окончена

builder.Services.AddAutoMapper(typeof(MappingProfile));
var app = builder.Build();

// ���������� ��������
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Strict,
    HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always,
    Secure = CookieSecurePolicy.SameAsRequest
});

// app.UseHttpsRedirection();

/*app.UseAuthorization();*/

app.MapControllers();

app.Run();
