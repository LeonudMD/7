using AuthService.Application.Contracts;
using AuthService.Application.Services;
using AuthService.Domain.Abstractions;
using AuthService.Domain.Models;
using AuthService.Infrastructure.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _service;
        private readonly IMapper _mapper;
        private readonly IKafkaProducerService _kafkaProducerService;
        public UsersController(IUserService service, 
            IMapper mapper, 
            IConfiguration configuration, 
            IKafkaProducerService kafkaProducerService) 
        {
            _service = service;
            _mapper = mapper;
            _configuration = configuration;
            _kafkaProducerService = kafkaProducerService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(RegisterUserRequest request)
        {
            await _service.RegisterUserAsync(request.username, request.password, request.email);
            await _kafkaProducerService.ProduceEmailRequestAsync(request.email, $"{request.username} Hui a ne is work");
            return Ok("Пользователь был успешно зарегистрирован!");
        }
        [HttpPost("login")]
        public async Task<ActionResult<object>> LoginUser(LoginUserRequest request)
        {
            var tokens = await _service.LoginUserAsync(request.email, request.password);
            Response.Cookies.Append("tasty-cookies", tokens.Item1);
            Response.Cookies.Append("refresh-cookies", tokens.Item2);
            return Ok(new {AccessToken = tokens.Item1, RefreshToken = tokens.Item2});
        }

        [HttpGet]
        public async Task<ActionResult<List<User>>> GetAllUsers()
        {
            var users = await _service.GetAllUsers();
            return Ok(users);
        }

        [HttpPost("logout")]
        public async Task<ActionResult<string>> Logout()
        {
            Response.Cookies.Delete("tasty-cookies");
            Response.Cookies.Delete("refresh-cookies");
            return Ok("Успешно вышел из аккаунта");
        }
        [HttpPost("refresh-token")]
        public async Task<ActionResult<object>> RefreshToken()
        {
            if (!Request.Cookies.TryGetValue("refresh-cookies", out var refreshToken))
            {
                return Unauthorized("RefreshToken не найден");
            }

            if(!Request.Cookies.TryGetValue("tasty-cookies", out var accessToken))
            {
                return Unauthorized("AccessToken не найден");
            }

            var result = GetPrincipalFromExpiredToken(accessToken);

            if(result == null)
            {
                return BadRequest("Некорректный AccessToken или RefreshToken");
            }

            var tokens = await _service.RefreshTokenAsync(refreshToken);
            Response.Cookies.Append("tasty-cookies", tokens.Item1);
            Response.Cookies.Append("refresh-cookies", tokens.Item2);

            return Ok(new { AccessToken = tokens.Item1, RefreshToken = tokens.Item2 });

        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtOptions:SecretKey"])),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        [HttpDelete]
        public async Task<ActionResult<string>> DeleteAllUsers()
        {
            await _service.DeleteAllUsers();

            return Ok("Пользователи удалены нахуй");
        }
    }
}


