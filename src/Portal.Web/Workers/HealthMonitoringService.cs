﻿using Microsoft.AspNetCore.SignalR;
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

        private readonly IHubContext<MonitoringHub, IMonitoringHub> _notifyHub;
        public HealthMonitoringService(IHubContext<MonitoringHub, IMonitoringHub> notifyHub)
        {

            _notifyHub = notifyHub;
        }

        public Task DoWork(CancellationToken stoppingToken)
        {
            var redis = ConnectionMultiplexer.Connect("localhost:6379");
            var db = redis.GetDatabase();

            var pubsub = redis.GetSubscriber();
            pubsub.Subscribe("health_monitor", async (channel, message) => await NotifyAction(message));
            return Task.CompletedTask;
        }

        private async Task NotifyAction(RedisValue message)
        {
            await _notifyHub.Clients.All.SendHealthUpdate(message);

        }
    }

    public interface IHealthMonitoringService
    {
        Task DoWork(CancellationToken stoppingToken);
    }
}