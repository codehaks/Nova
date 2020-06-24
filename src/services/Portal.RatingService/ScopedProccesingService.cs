using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Portal.RatingService.Data;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Portal.RatingService
{
    internal interface IScopedProcessingService
    {
        Task DoWork(CancellationToken stoppingToken);
    }



    internal class ScopedProcessingService : IScopedProcessingService
    {
        private readonly ILogger _logger;
        private readonly AppDbContext _db;

        public ScopedProcessingService(ILogger<ScopedProcessingService> logger, AppDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public async Task DoWork(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "post_rate", durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += Consumer_Received;

            channel.BasicConsume(queue: "post_rate", autoAck: true, consumer: consumer);

            await Task.CompletedTask;
        }

        private async void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {

            var body = e.Body;
            var message = Encoding.UTF8.GetString(body.ToArray());
            _logger.LogInformation(message);
            var model = JsonConvert.DeserializeObject<PostRating>(message);
            model.TimeCreated = DateTime.Now;
            var item = await _db.PostRatings.FirstOrDefaultAsync(p => p.PostId == model.PostId && p.UserId == model.UserId);
            if (item == null)
            {
                _db.PostRatings.Add(model);
            }
            else
            {
                item.Rate = model.Rate;
            }

            await _db.SaveChangesAsync();
            _logger.LogInformation("Save to database", model);

        }
    }
}
