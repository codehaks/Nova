using Grpc.Core.Logging;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Portal.Web.Hubs;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Portal.Web.Workers
{
    public class HealthMonitoringService : IHealthMonitoringService
    {

        private readonly IHubContext<MonitoringHub, IMonitoringHub> _monitoringHub;
        private readonly ILogger<HealthMonitoringWorker> _logger;
        public HealthMonitoringService(IHubContext<MonitoringHub, IMonitoringHub> monitoringHub, ILogger<HealthMonitoringWorker> logger)
        {

            _monitoringHub = monitoringHub;
            _logger = logger;
        }

        public Task DoWork(CancellationToken stoppingToken)
        {
            try
            {
                var redis = ConnectionMultiplexer.Connect("localhost:6379");
                var db = redis.GetDatabase();

                var pubsub = redis.GetSubscriber();
                pubsub.Subscribe("health_monitor", async (channel, message) => await NotifyAction(message));
            }
            catch (Exception)
            {

                _logger.LogError("Redis is not ready for health monitoring worker");
            }
  
            return Task.CompletedTask;
        }

        private async Task NotifyAction(RedisValue message)
        {
            await _monitoringHub.Clients.All.SendHealthUpdate(message);

        }
    }

    public interface IHealthMonitoringService
    {
        Task DoWork(CancellationToken stoppingToken);
    }
}
