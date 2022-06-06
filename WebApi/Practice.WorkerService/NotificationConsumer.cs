using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Practice.QueueProducers;
using Practice.Shared.Notifications;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using System.Text;

namespace Practice.WorkerService
{
    public class NotificationConsumer : IHostedService
    {
        private readonly Serilog.ILogger _logger;
        private readonly ConnectionFactory _connectionFactory;
        private readonly IServiceScopeFactory _scopeFactory;

        private readonly RabbitMQSettings _settings;

        public NotificationConsumer(IServiceScopeFactory scopeFactory, IOptions<RabbitMQSettings> settings)
        {
            _logger = Log.Logger;
            _scopeFactory = scopeFactory;

            _settings = settings.Value;

            _connectionFactory = new ConnectionFactory()
            {
                Uri = new Uri("amqp://guest:guest@localhost:5672")
            };
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Notification consumer running.");

            ReceiveNotification<CommitNotification>(_settings.CommitNotificationQueue);
            ReceiveNotification<UserChallengeNotification>(_settings.UserChallengeNotificationQueue);
            ReceiveNotification<AchievementNotification>(_settings.AchievementNotificationQueue);
            ReceiveNotification<VenueNotification>(_settings.VenueNotificationQueue);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.Information("UserChallenge Status Checker is stopping.");

            return Task.CompletedTask;
        }
        private Task ReceiveNotification<T>(string queue) where T : class, INotification
        {
            var rabbitMqConnection = _connectionFactory.CreateConnection();
            var rabbitMqChannel = rabbitMqConnection.CreateModel();

            rabbitMqChannel.QueueDeclare(queue: queue,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

            rabbitMqChannel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            var consumer = new EventingBasicConsumer(rabbitMqChannel);
            consumer.Received += (model, args) =>
            {
                var body = args.Body;
                var message = Encoding.UTF8.GetString(body.ToArray());

                var deserializedMessage = JsonConvert.DeserializeObject<Dictionary<string, T>>(message);
                var notification = deserializedMessage.First().Value;
                var receiver = deserializedMessage.First().Key;

                using (var scope = _scopeFactory.CreateScope())
                {
                    var notificationSender = scope.ServiceProvider.GetRequiredService<INotificationSender>();
                    notificationSender.SendAsync(notification, receiver);

                }
                rabbitMqChannel.BasicAck(deliveryTag: args.DeliveryTag, multiple: false);
            };
            rabbitMqChannel.BasicConsume(queue: queue,
                                         autoAck: false,
                                         consumer: consumer);

            return Task.CompletedTask;
        }
    }
}
