namespace EmailService.Domain.Abstractions;

public interface IKafkaProducerService
{
    Task ProduceEmailRequestAsync(string email, string message);
}