namespace Practice.Shared.Notifications
{
    public class CommitNotification : INotification
    {
        public Guid Id { get; init; }
        public string ChallengeName { get; init; }
        public string Body { get; init; }

        public CommitNotification(string challengeName, string status)
        {
            Id = Guid.NewGuid();
            ChallengeName = challengeName;
            Body = $"Status: {status}";
        }
    }
}
