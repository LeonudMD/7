using AuthService.Domain.Abstractions;

namespace AuthService.Application.Services
{
    public class KafkaProducerService : IKafkaProducerService
    {
        private readonly IKafkaProducer _kafkaProducer;

        public KafkaProducerService(IKafkaProducer kafkaProducer)
        {
            _kafkaProducer = kafkaProducer;
        }
        public async Task ProduceEmailRequestAsync(string email, string message)
        {
            await _kafkaProducer.ProduceEmailRequestAsync(email, message);
        }
    }
}
