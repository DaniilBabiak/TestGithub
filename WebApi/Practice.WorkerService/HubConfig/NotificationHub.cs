using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Practice.Shared.Notifications;

namespace Practice.WorkerService.HubConfig
{
    public class NotificationHub : Hub
    {
        [Authorize]
        public async Task SendNotificationToUser(INotification notification, Guid userId)
           => await Clients.User(userId.ToString()).SendAsync("transfernotification", notification);
    }
}
