using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Portal.Persistence;
using Portal.Web.Hubs;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Portal.Web.Workers
{
    public interface INotificationScopedService
    {
        Task DoWork(CancellationToken stoppingToken);
    }

    public class NotificationScopedService : INotificationScopedService
    {
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _db;
        private readonly IHubContext<NotificationHub, INotificationHub> _notifyHub;
        public NotificationScopedService(IHubContext<NotificationHub, INotificationHub> notifyHub,ILogger<NotificationScopedService> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
            _notifyHub = notifyHub;
        }

        public Task DoWork(CancellationToken stoppingToken)
        {
            var redis = ConnectionMultiplexer.Connect("localhost:6379");
            var db = redis.GetDatabase();

            var pubsub = redis.GetSubscriber();
            pubsub.Subscribe("post_notify", async (channel, message) => await NotifyActipn(message));
            return Task.CompletedTask;
        }

        private async Task NotifyActipn(RedisValue message)
        {
            await _notifyHub.Clients.All.SendNotification(message);

        }
    }
}
