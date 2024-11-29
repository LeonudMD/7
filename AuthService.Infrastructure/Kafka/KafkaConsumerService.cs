using System.Text.Json;
using AuthService.Domain.Models;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AuthService.Infrastructure.Kafka;

public class KafkaConsumerService : IHostedService
{
    private readonly ILogger<KafkaConsumerService> _logger;
    private readonly IConsumer<Ignore, string> _consumer;
    private readonly string _topic;
    private readonly IConfiguration _configuration;

    public KafkaConsumerService(ILogger<KafkaConsumerService> logger, IConfiguration configuration)
    {   
        _configuration = configuration;
        _logger = logger;
        

        var config = new ConsumerConfig
        {
            BootstrapServers = _configuration["Kafka:BootstrapServers"],
            GroupId = "consumer-group-auth",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        _topic = _configuration["Kafka:ResponseTopic"];
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Kafka Consumer started.");
        Task.Run(() => StartConsuming(cancellationToken));
        return Task.CompletedTask;
    }

    private void StartConsuming(CancellationToken cancellationToken)
    {
        _consumer.Subscribe(_topic);
        _logger.LogInformation("HHHHHHHHHHHHHHHHHHHHHASASASA");

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var consumeResult = _consumer.Consume(cancellationToken);
                var responseMessage = JsonSerializer.Deserialize<ResponseMessageModel>(consumeResult.Message.Value);

                if (responseMessage != null)
                {
                    _logger.LogInformation($"Получен ответ для email: {responseMessage.Email}, статус: {responseMessage.Status}, детали: {responseMessage.Details}");
                }
                else
                {
                    _logger.LogWarning("Не удалось десериализовать ответное сообщение.");
                }
            }
        }
        catch (OperationCanceledException)
        {
            
        }
        finally
        {
            _consumer.Close();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Kafka Consumer stopping.");
        _consumer.Close();
        return Task.CompletedTask;
    }
}