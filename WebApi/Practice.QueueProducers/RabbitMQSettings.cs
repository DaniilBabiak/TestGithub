namespace Practice.QueueProducers
{
    public class RabbitMQSettings
    {
        public string CommitNotificationQueue { get; init; }
        public string UserChallengeNotificationQueue { get; init; }
        public string AchievementNotificationQueue { get; init; }
        public string ChallengeNotificationQueue { get; init; }
        public string VenueNotificationQueue { get; init; }
    }
}
