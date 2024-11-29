using AuthService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.Abstractions
{
    public interface IUserService
    {
        Task<(string,string)> LoginUserAsync(string email, string password);
        Task RegisterUserAsync(string username, string password, string email);
        Task<List<User>> GetAllUsers();
        Task<(string, string)> RefreshTokenAsync(string refreshToken);
        Task DeleteAllUsers();
    }
}
