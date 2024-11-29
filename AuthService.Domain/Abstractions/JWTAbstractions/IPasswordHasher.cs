namespace AuthService.Infrastructure
{
    public interface IPasswordHasher
    {
        string GeneratePassword(string password);
        bool VerifyPassword(string password, string hashPassword);
    }
}