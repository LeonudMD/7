using AuthService.Domain.Models;

namespace AuthService.Infrastructure
{
    public interface IJwtProvider
    {
        string GenerateToken(User user);
        RefreshToken GenerateRefreshToken();
    }
}