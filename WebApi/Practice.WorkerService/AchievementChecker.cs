using Microsoft.EntityFrameworkCore;
using Practice.Data;
using Practice.Entities.Entities;
using Practice.QueueProducers.Interfaces;
using Practice.Shared.Notifications;
using Serilog;

namespace Practice.WorkerService
{
    public class AchievementChecker : IHostedService
    {
        private int executionCount = 0;
        private Timer _timer = null!;
        private readonly Serilog.ILogger _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public AchievementChecker(IServiceScopeFactory scopeFactory)
        {
            _logger = Log.Logger;
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.Information("Achievement Checker running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(60));

            return Task.CompletedTask;
        }

        private async void DoWork(object? state)
        {
            await CheckAchievementsAsync();

            var count = Interlocked.Increment(ref executionCount);
            _logger.Information(
                "Achievement Checker is working. Count: {Count}", count);
        }

        private async Task CheckAchievementsAsync()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DBContext>();

                var users = context.Users.Include(u => u.Achievements);
                var achievements = context.Achievements;

                foreach (var user in users)
                {
                    foreach (var achievement in achievements)
                    {
                        var completedChallenges = context.UserChallenges.Where(c => c.UserId == user.Id
                                                                            && c.Status == UserChallengeStatuses.Completed)
                                                                        .Include(u => u.Challenge);
                        var count = 0;

                        foreach (var completedChallenge in completedChallenges)
                        {
                            if (completedChallenge.Challenge.TypeId == achievement.ChallengeTypeId)
                            {
                                count++;
                            }
                        }

                        if (count >= achievement.Streak)
                        {
                            if (user.Achievements.FirstOrDefault(ua => ua.Id == achievement.Id) == null)
                            {
                                user.Achievements.Add(achievement);

                                var notification = new AchievementNotification(achievement.Name);

                                var notificationProducer = scope.ServiceProvider.GetRequiredService<INotificationProducer>();
                                notificationProducer.Send(notification, user.Id.ToString());
                            }
                        }

                    }
                }
                await context.SaveChangesAsync();
            }
        }
        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.Information("Achievement Checker is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
