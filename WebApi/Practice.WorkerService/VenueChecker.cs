using Practice.Data;
using Practice.QueueProducers.Interfaces;
using Practice.Shared.Notifications;
using Serilog;
using System.Data.Entity;

namespace Practice.WorkerService
{
    public class VenueChecker : IHostedService
    {
        private int executionCount = 0;
        private Timer _timer = null!;
        private readonly Serilog.ILogger _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public VenueChecker(IServiceScopeFactory scopeFactory)
        {
            _logger = Log.Logger;
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.Information("Venue Checker running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromDays(1));

            return Task.CompletedTask;
        }

        private async void DoWork(object? state)
        {
            Task[] tasks =
                {
                    RemoveExpiredVenues(),
                    RemindUser()
                };

            await Task.WhenAll(tasks);

            var count = Interlocked.Increment(ref executionCount);
            _logger.Information(
                "Venue Checker is working. Count: {Count}", count);
        }

        private Task RemindUser()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DBContext>();
                var notificationProducer = scope.ServiceProvider.GetRequiredService<INotificationProducer>();

                var venues = context.Venues.Include(v => v.Attendees);

                foreach (var venue in venues)
                {
                    if (venue.Date - DateTime.UtcNow < TimeSpan.FromDays(1))
                    {
                        if (venue.Attendees != null)
                        {
                            foreach (var attendee in venue.Attendees)
                            {
                                var notification = new VenueNotification($"You have venue tomorrow: {venue.Title}.");
                                notificationProducer.Send(notification, attendee.Id.ToString());
                            }
                        }
                    }
                }
            }

            return Task.CompletedTask;
        }

        private Task RemoveExpiredVenues()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DBContext>();

                foreach (var venue in context.Venues)
                {
                    if (venue.Date < DateTime.UtcNow)
                    {
                        context.Remove(venue);
                    }
                }
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.Information("Venue Checker is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
