using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Portal.Web.Workers
{
    public class HealthMonitoringWorker : BackgroundService
    {
        private readonly ILogger<HealthMonitoringWorker> _logger;

        public IServiceProvider Services { get; }

        public HealthMonitoringWorker(ILogger<HealthMonitoringWorker> logger, IServiceProvider services)
        {
            _logger = logger;
            Services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await DoWork(stoppingToken);
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Health Monitoring worker is running.");

            var scope = Services.CreateScope();

            var scopedProcessingService =
                scope.ServiceProvider
                    .GetRequiredService<IHealthMonitoringService>();

            await scopedProcessingService.DoWork(stoppingToken);
        }
    }
}
