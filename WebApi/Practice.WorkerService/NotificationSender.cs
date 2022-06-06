using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Practice.Entities.Entities;
using Practice.Shared.Notifications;
using Practice.WorkerService.HubConfig;
using Serilog;

namespace Practice.WorkerService
{
    public class NotificationSender : INotificationSender
    {
        private readonly Serilog.ILogger _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public NotificationSender(IServiceScopeFactory scopeFactory)
        {
            _logger = Log.Logger;
            _scopeFactory = scopeFactory;
        }

        public async Task SendAsync(INotification notification, string receiver)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var hub = scope.ServiceProvider.GetRequiredService<IHubContext<NotificationHub>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

                var user = await userManager.Users.Include(u => u.NotificationsSettings).FirstAsync(u => u.Id.ToString() == receiver);

                bool isSubscribed;

                switch (notification.GetType().Name)
                {
                    case "AchievementNotification":
                        {
                            isSubscribed = user.NotificationsSettings.SendAchievementNotification;
                            break;
                        }
                    case "UserChallengeNotification":
                        {
                            isSubscribed = user.NotificationsSettings.SendUserChallengeNotification;
                            break;
                        }
                    case "CommitNotification":
                        {
                            isSubscribed = user.NotificationsSettings.SendCommitNotification;
                            break;
                        }
                    case "ChallengeNotification":
                        {
                            isSubscribed = user.NotificationsSettings.SendChallengeNotification;
                            break;
                        }
                    case "VenueNotification":
                        {
                            isSubscribed = user.NotificationsSettings.SendVenueNotification;
                            break;
                        }
                    default:
                        {
                            isSubscribed = false;
                            break;
                        }
                }
                if (isSubscribed)
                {
                    await hub.Clients.All.SendAsync("transfernotification/" + receiver, notification);
                }
            }
        }
    }
}
