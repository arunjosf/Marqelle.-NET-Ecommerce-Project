using Microsoft.Extensions.Configuration; 
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Marqelle.Application.DTO;
using Marqelle.Application.Interfaces;

namespace Marqelle.Api.BackgroundServices
{
    public class EmailConsumerService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private IConnection _connection;
        private RabbitMQ.Client.IModel _channel;

        public EmailConsumerService(IServiceProvider serviceProvider, IConfiguration config)
        {
            _serviceProvider = serviceProvider;

            string rabbitUrl = config["RabbitMQ_Url"];

            var factory = new ConnectionFactory { Uri = new Uri(rabbitUrl) };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "email_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
        }


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                var emailMessage = JsonSerializer.Deserialize<EmailMessageDto>(json);

                using (var scope = _serviceProvider.CreateScope())
                {
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                    await emailService.SendOtpEmailAsync(emailMessage.ToEmail, emailMessage.Body);
                }
            };

            _channel.BasicConsume(queue: "email_queue", autoAck: true, consumer: consumer);
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}