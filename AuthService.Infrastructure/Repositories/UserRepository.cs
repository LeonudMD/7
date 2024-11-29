using AuthService.Domain.Abstractions;
using AuthService.Domain.Models;
using AuthService.Infrastructure.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UsersDbContext _context;
        private readonly IMapper _mapper;
        public UserRepository(UsersDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task Add(User user)
        {
            var userEntity = new UserEntity
            {
                UserName = user.UserName,
                Email = user.Email,
                PasswordHash = user.PasswordHash,
                Role = user.Role,
                RefreshToken = "22",
                RefreshTokenExpiration = DateTime.UtcNow
            };
            await _context.AddAsync(userEntity);
            await _context.SaveChangesAsync();
        }

        public async Task AddRefreshTokenAsync(User user)
        {
            await _context._users
                .Where(t => t.Id == user.Id)
                .ExecuteUpdateAsync(s => s
                .SetProperty(t => t.RefreshToken, t => user.RefreshToken)
                .SetProperty(t => t.RefreshTokenExpiration, t => user.RefreshTokenExpiration));
        }
        public async Task<User> GetByEmail(string email)
        {
            var userEntity = await _context._users
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Email == email);

            return _mapper.Map<User>(userEntity);
        }

        public async Task<User> GetByRefreshToken(string refreshToken)
        {
            var user = await _context._users
                .AsNoTracking()
                .FirstOrDefaultAsync(t=>t.RefreshToken == refreshToken);
            return _mapper.Map<User>(user);
        }

        public async Task<List<User>> GetAll()
        {
            var users = await _context._users.AsNoTracking().ToListAsync();

            return users.Select(x => _mapper.Map<User>(x)).ToList();
        }

        public async Task DeleteAll()
        {
            _context._users.RemoveRange(_context._users);

            await _context.SaveChangesAsync();
        }
    }
}
