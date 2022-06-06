namespace Practice.Shared.Notifications
{
    public class AchievementNotification : INotification
    {
        public Guid Id { get; init; }
        public string Body { get; init; }
        public AchievementNotification(string name)
        {
            Id = Guid.NewGuid();
            Body = name;
        }
    }
}
