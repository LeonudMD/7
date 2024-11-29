namespace AuthService.Domain.Abstractions;

public interface IKafkaProducer
{
    Task ProduceEmailRequestAsync(string email, string message);
}