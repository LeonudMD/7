using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure
{
    public class PasswordHasher : IPasswordHasher
    {
        public string GeneratePassword(string password)
        {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(password);
        }
        public bool VerifyPassword(string password, string hashPassword)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(password, hashPassword);
        }
    }
}
