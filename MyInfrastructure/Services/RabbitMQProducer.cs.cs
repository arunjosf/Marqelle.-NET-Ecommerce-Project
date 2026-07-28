using Marqelle.Application.DTO;
using Microsoft.Extensions.Configuration; 
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using static Marqelle.Application.Services.UserAuthService;

namespace Marqelle.Infrastructure.Services
{
    public class RabbitMQProducer : IRabbitMQProducer
    {
        private readonly IConfiguration _config;

        public RabbitMQProducer(IConfiguration config)
        {
            _config = config;
        }

        public void SendEmailMessage(EmailMessageDto message)
        {
            string rabbitUrl = _config["RabbitMQ_Url"];

            var factory = new ConnectionFactory { Uri = new Uri(rabbitUrl) };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "email_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            channel.BasicPublish(exchange: "", routingKey: "email_queue", basicProperties: null, body: body);
        }
    }
}