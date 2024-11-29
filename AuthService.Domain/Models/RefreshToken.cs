namespace AuthService.Domain.Models
{
    public class RefreshToken
    {
        public string Value { get; set; }
        public DateTime DateExpiration { get; set; }
    }
}
