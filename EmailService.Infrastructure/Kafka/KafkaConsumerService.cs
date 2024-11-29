using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using EmailService.Domain.Abstractions;
using EmailService.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace EmailService.Infrastructure.Kafka
{
    public class KafkaConsumerService : IHostedService
    {
        private readonly ILogger<KafkaConsumerService> _logger;
        private readonly IConsumer<Ignore, string> _consumer;
        private readonly string _topic;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        public KafkaConsumerService(
            ILogger<KafkaConsumerService> logger,
            IConfiguration configuration,
            IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _logger = logger;
            _serviceProvider = serviceProvider;

            var config = new ConsumerConfig
            {
                BootstrapServers = _configuration["Kafka:BootstrapServers"],
                GroupId = _configuration["Kafka:GroupId"],
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            _topic = _configuration["Kafka:UserRegisteredTopic"];
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Kafka Consumer started.");
            return Task.Run(() => StartConsuming(cancellationToken), cancellationToken);
        }

        private async Task StartConsuming(CancellationToken cancellationToken)
        {
            _consumer.Subscribe(_topic);
            _logger.LogInformation($"Subscribed to topic: {_topic}");

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = _consumer.Consume(cancellationToken);
                        var message = JsonSerializer.Deserialize<MessageModel>(consumeResult.Message.Value);

                        if (message != null)
                        {
                            _logger.LogInformation($"Получено сообщение для email: {message.Email}, с текстом: {message.Message}");

                            using (var scope = _serviceProvider.CreateScope())
                            {
                                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                                var success = await emailService.SendWelcomeEmailAsync(message.Email, message.Message);
                                _logger.LogInformation($"success: {success}");

                                if (success)
                                {
                                    var kafkaProducer = scope.ServiceProvider.GetRequiredService<IKafkaProducer>();
                                    await kafkaProducer.ProduceEmailRequestAsync(message.Email, "Email успешно отправлен.");
                                    _logger.LogInformation($"Подтверждение отправлено для email: {message.Email}");
                                }
                                else
                                {
                                    _logger.LogWarning($"Не удалось отправить email: {message.Email}");
                                }
                            }
                        }
                        else
                        {
                            _logger.LogWarning("Не удалось десериализовать сообщение.");
                        }
                    }
                    catch (ConsumeException ex)
                    {
                        _logger.LogError($"Ошибка при потреблении сообщения: {ex.Error.Reason}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Consuming operation canceled.");
            }
            finally
            {
                _consumer.Close();
                _logger.LogInformation("Kafka Consumer closed.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Kafka Consumer stopping.");
            _consumer.Close();
            return Task.CompletedTask;
        }
    }
}
