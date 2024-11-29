using AuthService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.Abstractions
{
        public interface IUserRepository
        {
            Task Add(User user);
            Task<User> GetByEmail(string email);
            Task AddRefreshTokenAsync(User user);
            Task<List<User>> GetAll();
            Task<User> GetByRefreshToken(string refreshToken);
            Task DeleteAll();
        }
}
