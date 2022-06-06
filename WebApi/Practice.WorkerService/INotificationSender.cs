using Practice.Shared.Notifications;

namespace Practice.WorkerService
{
    public interface INotificationSender
    {
        Task SendAsync(INotification notification, string receiver);
    }
}
