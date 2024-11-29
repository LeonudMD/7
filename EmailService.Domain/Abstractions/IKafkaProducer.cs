namespace EmailService.Domain.Abstractions;

public interface IKafkaProducer
{
    Task ProduceEmailRequestAsync(string email, string message);
}

public interface IEmailService
{
    Task<bool> SendWelcomeEmailAsync(string recipientEmail, string message);
}