namespace Practice.Shared.Notifications
{
    public class ChallengeNotification : INotification
    {
        public Guid Id { get; init; }
        public string ChallengeName { get; init; }
        public string Body { get; init; }

        public ChallengeNotification(string challengeName, string message)
        {
            Id = Guid.NewGuid();
            ChallengeName = challengeName;
            Body = message;
        }
    }
}
