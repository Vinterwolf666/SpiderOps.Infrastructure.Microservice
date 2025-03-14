using RabbitMQ.Client;
using System.Text;

namespace Infrastructure.Microservice.API.Services
{
    public class RabbitMQProducer
    {
        private const string QueueName = "infrastructure_created_stage";


        private readonly ConnectionFactory _factory;

        public RabbitMQProducer()
        {
            _factory = new ConnectionFactory()
            {
                HostName = "35.202.158.223",
                UserName = "vinterwolf",
                Password = "vinterland"
            };

        }
        public async Task InfrastrctureStageComplete()
        {
            await using var connection = await _factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: QueueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var message = "Infrastructure_stage_complete";
            var body = Encoding.UTF8.GetBytes(message);
            var properties = new BasicProperties();

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: QueueName,
                mandatory: false,
                basicProperties: properties,
                body: body
            );

            Console.WriteLine($"[x] Sent: {message}");
        }

    }
}
