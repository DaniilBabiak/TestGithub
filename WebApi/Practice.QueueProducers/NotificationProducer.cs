using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Practice.QueueProducers.Interfaces;
using Practice.Shared.Notifications;
using RabbitMQ.Client;
using System.Text;

namespace Practice.QueueProducers
{
    public class NotificationProducer : INotificationProducer
    {
        private readonly ConnectionFactory _connectionFactory;
        private readonly RabbitMQSettings _settings;

        public NotificationProducer(IOptions<RabbitMQSettings> settings)
        {
            _settings = settings.Value;

            _connectionFactory = new ConnectionFactory()
            {
                Uri = new Uri("amqp://guest:guest@localhost:5672")
            };
        }
        public void Send(CommitNotification notification, string receiver)
        {
            Send(notification, receiver, _settings.CommitNotificationQueue);
        }

        public void Send(UserChallengeNotification notification, string receiver)
        {
            Send(notification, receiver, _settings.UserChallengeNotificationQueue);
        }

        public void Send(AchievementNotification notification, string receiver)
        {
            Send(notification, receiver, _settings.AchievementNotificationQueue);
        }

        public void Send(ChallengeNotification notification, string receiver)
        {
            Send(notification, receiver, _settings.ChallengeNotificationQueue);
        }

        public void Send(VenueNotification notification, string receiver)
        {
            Send(notification, receiver, _settings.VenueNotificationQueue);
        }

        private void Send(INotification notification, string receiver, string queue)
        {
            var message = new Dictionary<string, INotification>();
            message.Add(receiver, notification);

            using (var connection = _connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queue,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                string serializedMessage = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(serializedMessage);

                channel.BasicPublish(exchange: "",
                                     routingKey: queue,
                                     basicProperties: null,
                                     body: body);
            }
        }
    }
}
