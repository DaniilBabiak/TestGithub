namespace Practice.Shared.Notifications
{
    public interface INotification
    {
        public Guid Id { get; }
        public string Body { get; }
    }
}