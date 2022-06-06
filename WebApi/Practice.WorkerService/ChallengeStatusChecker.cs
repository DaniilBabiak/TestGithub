using Practice.Data;
using Practice.Entities.Entities;
using Practice.QueueProducers.Interfaces;
using Practice.Shared.Notifications;
using Serilog;
namespace Practice.WorkerService
{
    public class ChallengeStatusChecker : IHostedService
    {
        private int executionCount = 0;
        private Timer _timer = null!;
        private readonly Serilog.ILogger _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public ChallengeStatusChecker(IServiceScopeFactory scopeFactory)
        {
            _logger = Log.Logger;
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.Information("Challenge Status Checker running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(60));

            return Task.CompletedTask;
        }

        private async void DoWork(object? state)
        {
            Task[] tasks =
                {
                    DisableChallengesAsync(),
                    EnableChallengesAsync()
                };

            await Task.WhenAll(tasks);

            var count = Interlocked.Increment(ref executionCount);
            _logger.Information(
                "Challenge Status Checker is working. Count: {Count}", count);
        }

        private async Task EnableChallengesAsync()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DBContext>();

                var challenges = context.Challenges.Where(c => c.AvailableFrom <= DateTime.UtcNow
                                                        && c.AvailableTo >= DateTime.UtcNow
                                                        && c.Status != ChallengeStatuses.Enabled);

                foreach (var challenge in challenges)
                {
                    challenge.Status = ChallengeStatuses.Enabled;

                    var notification = new ChallengeNotification(challenge.Name, "A new challenge is available."); ;

                    var notificationProducer = scope.ServiceProvider.GetRequiredService<INotificationProducer>();
                    notificationProducer.Send(notification, "all");
                }
                context.UpdateRange(challenges);
                await context.SaveChangesAsync();
            }
        }

        private async Task DisableChallengesAsync()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DBContext>();

                var expiredChallenges = context.Challenges.Where(c => c.AvailableTo < DateTime.UtcNow
                                                               && c.Status != ChallengeStatuses.Disabled);

                foreach (var challenge in expiredChallenges)
                {
                    challenge.Status = ChallengeStatuses.Disabled;
                }

                context.UpdateRange(expiredChallenges);
                await context.SaveChangesAsync();
            }
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.Information("Challenge Status Checker is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}