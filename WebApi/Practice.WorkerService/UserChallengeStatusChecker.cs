using Microsoft.EntityFrameworkCore;
using Practice.Data;
using Practice.Entities.Entities;
using Practice.QueueProducers.Interfaces;
using Practice.Shared.Notifications;
using Serilog;

namespace Practice.WorkerService
{
    public class UserChallengeStatusChecker : IHostedService
    {
        private int executionCount = 0;
        private Timer _timer = null!;
        private readonly Serilog.ILogger _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public UserChallengeStatusChecker(IServiceScopeFactory scopeFactory)
        {
            _logger = Log.Logger;
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.Information("UserChallenge Status Checker running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(60));

            return Task.CompletedTask;
        }

        private async void DoWork(object? state)
        {
            await EndUserChallengesAsync();

            var count = Interlocked.Increment(ref executionCount);
            _logger.Information(
                "UserChallenge Status Checker is working. Count: {Count}", count);
        }

        private async Task EndUserChallengesAsync()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DBContext>();

                var userChallenges = context.UserChallenges
                                            .Include(u => u.Challenge)
                                            .Where(c => c.Challenge.Status == ChallengeStatuses.Disabled
                                                               && c.Status == UserChallengeStatuses.Started);
                foreach (var userChallenge in userChallenges)
                {
                    userChallenge.Status = UserChallengeStatuses.Expired;
                    var notification = new UserChallengeNotification(userChallenge.Challenge.Name,
                                                                     userChallenge.Status.ToString());

                    var notificationProducer = scope.ServiceProvider.GetRequiredService<INotificationProducer>();
                    notificationProducer.Send(notification, userChallenge.UserId.ToString());
                }
                context.UpdateRange(userChallenges);
                await context.SaveChangesAsync();

            }
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.Information("UserChallenge Status Checker is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
