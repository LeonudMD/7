using System.Text.Json;
using AuthService.Domain.Abstractions;
using AuthService.Domain.Models;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

namespace AuthService.Infrastructure;

public class KafkaProducer : IKafkaProducer
{
    private readonly IProducer<Null, string> _producer;
    private readonly string? _topic;

    public KafkaProducer(IConfiguration configuration, IProducer<Null, string> producer)
    {
        _producer = producer;
        _topic = configuration["Kafka:UserRegisteredTopic"];
    }

    public async Task ProduceEmailRequestAsync(string email, string message)
    {
        var mes = new MessageModel
        {
            Email = email,
            Message = message
        };
        var jsonMessage = JsonSerializer.Serialize(mes);
        await _producer.ProduceAsync(_topic, new Message<Null, string> { Value = jsonMessage });
    }
}