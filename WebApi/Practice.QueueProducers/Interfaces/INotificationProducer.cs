using Practice.Shared.Notifications;

namespace Practice.QueueProducers.Interfaces
{
    public interface INotificationProducer
    {
        public void Send(CommitNotification notification, string receiver);
        public void Send(UserChallengeNotification notification, string receiver);
        public void Send(AchievementNotification notification, string receiver);
        public void Send(ChallengeNotification notification, string receiver);
        public void Send(VenueNotification notification, string receiver);
    }
}
