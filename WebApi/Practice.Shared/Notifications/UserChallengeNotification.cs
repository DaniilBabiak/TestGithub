namespace Practice.Shared.Notifications
{
    public class UserChallengeNotification : INotification
    {
        public Guid Id { get; init; }
        public string ChallengeName { get; init; }
        public string Body { get; init; }

        public UserChallengeNotification(string challengeName, string status)
        {
            Id = Guid.NewGuid();
            ChallengeName = challengeName;
            Body = $"Status: {status}";
        }
    }
}
