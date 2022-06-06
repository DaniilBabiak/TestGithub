namespace Practice.Shared.Notifications
{
    public class VenueNotification : INotification
    {
        public Guid Id { get; init; }
        public string Body { get; init; }
        public VenueNotification(string message)
        {
            Id = Guid.NewGuid();
            Body = message;
        }
    }
}
