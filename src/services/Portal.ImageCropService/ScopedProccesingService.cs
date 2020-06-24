using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Portal.ImageCropService.Data;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using StackExchange.Redis;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Portal.ImageCropService
{
    internal interface IScopedProcessingService
    {
        Task DoWork(CancellationToken stoppingToken);
    }



    internal class ScopedProcessingService : IScopedProcessingService
    {
        private readonly ILogger _logger;
        private readonly ImageDbContext _db;

        public ScopedProcessingService(ILogger<ScopedProcessingService> logger, ImageDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public async Task DoWork(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "image_crop", durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += Consumer_Received;

            channel.BasicConsume(queue: "image_crop", autoAck: true, consumer: consumer);

            await Task.CompletedTask;
        }

        private async void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {

            var body = e.Body;
            var postId = Encoding.UTF8.GetString(body.ToArray());
            _logger.LogInformation(postId);
            
            var image = await _db.Images.FindAsync(Guid.Parse(postId));

            var stream = new MemoryStream();

            using (var pic = SixLabors.ImageSharp.Image.Load(image.Full))
            {
                pic.Mutate(x => x
                     .Resize(200, 200));

                //pic.SaveAsJpeg(stream);
                stream.Position = 0;

                pic.SaveAsJpeg(stream);

            }

            stream.Position = 0;
            image.Thumb = stream.ToArray();

            await Task.Delay(3000);

            await _db.SaveChangesAsync();
            _logger.LogInformation("Save to database -> PostId :", postId);


            var redis = ConnectionMultiplexer.Connect("localhost:6379");
            var db = redis.GetDatabase();

            var pubsub = redis.GetSubscriber();
            pubsub.Publish("post_notify",new RedisValue("Post is ready!"));

        }
    }
}
