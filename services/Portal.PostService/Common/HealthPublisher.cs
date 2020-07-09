using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Portal.PostService.Common
{
    public class HealthPublisher : IHealthCheckPublisher
    {
        private readonly ILogger<HealthPublisher> _logger;

        public HealthPublisher(ILogger<HealthPublisher> logger)
        {
            _logger = logger;
        }

        public Task PublishAsync(HealthReport report, CancellationToken cancellationToken)
        {
            var redis = ConnectionMultiplexer.Connect("localhost:6379");
            var pubsub = redis.GetSubscriber();
            _logger.LogInformation("Published : " + report.Status);
            pubsub.Publish("health_monitor", new RedisValue("PostService:"+report.Status));
            return Task.CompletedTask;
        }
    }
}
