using System.Text.Json;
using Confluent.Kafka;
using EmailService.Domain.Abstractions;
using Microsoft.Extensions.Configuration;
namespace EmailService.Infrastructure.Kafka;

public class KafkaProducer : IKafkaProducer
{
    private readonly IProducer<Null, string> _producer;
    private readonly string? _topic;

    public KafkaProducer(IConfiguration configuration, IProducer<Null, string> producer)
    {
        _producer = producer;
        _topic = configuration["Kafka:ResponseTopic"];
    }

    public async Task ProduceEmailRequestAsync(string email, string message)
    {
        var responseMessage = new
        {
            Email = email,
            Status = "Email sent",
            Details = message
        };

        var jsonMessage = JsonSerializer.Serialize(responseMessage);

        await _producer.ProduceAsync(_topic, new Message<Null, string> { Value = jsonMessage });
    }
}