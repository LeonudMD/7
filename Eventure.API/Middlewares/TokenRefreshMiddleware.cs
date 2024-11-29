using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Eventure.API.Middlewares
{
    public class TokenRefreshMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<TokenRefreshMiddleware> _logger;

        public TokenRefreshMiddleware(RequestDelegate next, IConfiguration configuration, HttpClient httpClient, ILogger<TokenRefreshMiddleware> logger)
        {
            _httpClient = httpClient;
            _next = next;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Cookies.TryGetValue("tasty-cookies", out var accessToken))
            {
                if (!IsTokenExpired(accessToken))
                {
                    if (context.Request.Cookies.TryGetValue("refresh-cookies", out var refreshToken))
                    {
                        // Создайте запрос с передачей куки
                        var request = new HttpRequestMessage(HttpMethod.Post, "http://authservice.api:8082/api/Users/refresh-token");

                        // Передайте куки из текущего запроса в новый запрос
                        request.Headers.Add("Cookie", $"tasty-cookies={accessToken}; refresh-cookies={refreshToken}");
                            var response = await _httpClient.SendAsync(request);


                            if (response.IsSuccessStatusCode)
                            {
                                string responseBody = await response.Content.ReadAsStringAsync();
                                _logger.LogInformation(responseBody);

                                // Обновите куки, если получили новые токены
                                var tokens = System.Text.Json.JsonSerializer.Deserialize<TokensResponse>(responseBody);
                            if (tokens != null && !string.IsNullOrEmpty(tokens.AccessToken) && !string.IsNullOrEmpty(tokens.RefreshToken))
                            {
                                // Добавляем куки с параметрами безопасности
                                var cookieOptions = new CookieOptions
                                {
                                    HttpOnly = true,
                                    Secure = true,
                                    Expires = DateTimeOffset.UtcNow.AddMinutes(30)
                                };

                                context.Response.Cookies.Append("tasty-cookies", tokens.AccessToken, cookieOptions);
                                context.Response.Cookies.Append("refresh-cookies", tokens.RefreshToken, cookieOptions);
                            }
                        }
                            else
                            {
                                _logger.LogError("Ошибка при выполнении запроса: " + response.ReasonPhrase);
                            }
                        }
                    }
                }
           

            await _next(context);
        }

        private bool IsTokenExpired(string token)
        {
            var lifetime = new JwtSecurityTokenHandler().ReadToken(token).ValidTo;
            return lifetime < DateTime.UtcNow;
        }

        // Класс для десериализации ответа
        public class TokensResponse
        {
            [System.Text.Json.Serialization.JsonPropertyName("accessToken")]
            public string AccessToken { get; set; }

            [System.Text.Json.Serialization.JsonPropertyName("refreshToken")]
            public string RefreshToken { get; set; }
        }

    }
}