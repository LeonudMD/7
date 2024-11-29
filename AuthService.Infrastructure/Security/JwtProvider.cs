using AuthService.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure
{
    public class JwtProvider : IJwtProvider
    {
        private readonly IConfiguration _configuragion;
        private readonly JwtOptions _options;

        public JwtProvider(IOptions<JwtOptions> options, IConfiguration configuration)
        {

            _configuragion = configuration;
            _options = new JwtOptions
            {
                SecretKey = _configuragion["JwtOptions:SecretKey"],
                ExpiresHours = int.Parse(_configuragion["JwtOptions:ExpiresHours"]),
                ExpiresRefreshDays = int.Parse(_configuragion["JwtOptions:ExpiresDaysRefresh"]),
                Issuer = _configuragion["JwtOptions:Issuer"],
                Audience = _configuragion["JwtOptions:Audience"]
            };
        }

        public string GenerateToken(User user)
        {
            Claim[] claims = [
                new("userid", user.Id.ToString()),
                new(ClaimTypes.Role, user.Role)
                ];

            var signingCred = new
                SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)), SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
                (
                    claims: claims,
                    signingCredentials: signingCred,
                    expires: DateTime.UtcNow.AddHours(_options.ExpiresHours),
                    issuer: _options.Issuer,
                    audience: _options.Audience
                );

            var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenValue;
        }
        public RefreshToken GenerateRefreshToken()
        {
            var refreshToken = Guid.NewGuid();

            return new RefreshToken
            {
                Value = refreshToken.ToString(),
                DateExpiration = DateTime.UtcNow.AddDays(_options.ExpiresRefreshDays)
            };
        }
    }
}
