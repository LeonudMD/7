using Eventure.API.Middlewares;
using Eventure.Application.MappingProfiles;
using Eventure.Application.Validators;
using Eventure.DB;
using Eventure.DB.Repositories;
using Eventure.Domain.Absctrations;
using Eventure.Domain.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services
    .AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters();

builder.Services.AddHttpClient();
builder.Services.AddLogging(); 

builder.Services.AddValidatorsFromAssemblyContaining<TicketRequestValidator>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddDbContext(builder.Configuration);

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8081, listenOptions =>
    {
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2;
    });
});


builder.Services.AddStackExchangeRedisCache
    (
        options =>
        {
            options.Configuration = builder.Configuration.GetConnectionString("EventureDbRedis");
            options.InstanceName = "local";
        }
    );

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtOptions:SecretKey"])),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = builder.Configuration["JwtOptions:Issuer"],
        ValidAudience = builder.Configuration["JwtOptions:Audience"]
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Попробуем взять токен из заголовка Authorization
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (!string.IsNullOrEmpty(token))
            {
                context.Token = token;
                context.HttpContext.RequestServices
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("JWT Authentication")
                    .LogInformation("Token взят из заголовка Authorization");
            }
            else if (context.Request.Cookies.TryGetValue("tasty-cookies", out token))
            {
                // Если в заголовке Authorization нет токена, попробуем взять его из куки
                context.Token = token;
                context.HttpContext.RequestServices
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("JWT Authentication")
                    .LogInformation("Token взят из куки tasty-cookies");
            }
            else
            {
                context.HttpContext.RequestServices
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("JWT Authentication")
                    .LogWarning("Token не найден ни в заголовке, ни в куках");
            }

            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization
    (options => 
    {
        options.AddPolicy("AdminOnly", policy => policy.RequireRole("admin"));
        options.AddPolicy("UserOnly", policy => policy.RequireRole("user")); 
    }
);


var app = builder.Build();

// ?????????? ????????
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.None,
    HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always,
    Secure = CookieSecurePolicy.SameAsRequest 
});

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<TokenRefreshMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
