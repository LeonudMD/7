using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.Entities
{
    public class UserEntity
    {
        public int Id { get; set; }
        public string? UserName { get; set; }

        public string? PasswordHash { get; set; }

        public string? Email { get; set; }

        public string? Role { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiration { get; set; } 
    }
}
