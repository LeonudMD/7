using AuthService.Domain.Abstractions;
using AuthService.Domain.Models;
using AuthService.Infrastructure;

namespace AuthService.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IPasswordHasher _hasher;
        private readonly IUserRepository _userRepository;
        private readonly IJwtProvider _jwtProvider;
        public UserService(IPasswordHasher hasher, IUserRepository userRepository, IJwtProvider jwtProvider)
        {
            _hasher = hasher;
            _userRepository = userRepository;
            _jwtProvider = jwtProvider;
        }
        public async Task RegisterUserAsync(string username, string password, string email)
        {
            var hashedPassword = _hasher.GeneratePassword(password);
            var user = new User
            {
                UserName = username,
                Email = email,
                PasswordHash = hashedPassword,
                Role = "admin"
            };

            await _userRepository.Add(user);
        }

        public async Task<(string,string)> LoginUserAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmail(email);

            var result = _hasher.VerifyPassword(password, user.PasswordHash);

            if (!result)
            {
                throw new UnauthorizedAccessException("Неверный логин или пароль");
            }

            var token = _jwtProvider.GenerateToken(user);
            var refreshToken = _jwtProvider.GenerateRefreshToken();

            user.RefreshToken = refreshToken.Value;
            user.RefreshTokenExpiration = refreshToken.DateExpiration;

            await _userRepository.AddRefreshTokenAsync(user);

            return (token, refreshToken.Value);
        }

        public async Task<(string, string)> RefreshTokenAsync(string refreshToken)
        {
            var user = await _userRepository.GetByRefreshToken(refreshToken);

            if(user == null || user.RefreshTokenExpiration < DateTime.Now)
            {
                throw new UnauthorizedAccessException("Неверный Refresh Token");
            }

            var newAccessToken = _jwtProvider.GenerateToken(user);

            var newRefreshToken = _jwtProvider.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken.Value;

            user.RefreshTokenExpiration = newRefreshToken.DateExpiration;

            await _userRepository.AddRefreshTokenAsync(user);

            return (newAccessToken, newRefreshToken.Value);
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _userRepository.GetAll();
        }

        public async Task DeleteAllUsers()
        {
            await _userRepository.DeleteAll();
        }

    }
}
