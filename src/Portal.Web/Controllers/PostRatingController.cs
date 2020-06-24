using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Portal.Web.Common;
using RabbitMQ.Client;

namespace Portal.Web.Controllers
{

    //[ApiController]
    //[Route("api/post")]
    public class PostRatingController : Controller
    {
        [Route("api/post/test")]
        public IActionResult Get()
        {
            return Ok("Done!");
        }

        [HttpPost]
        [Route("api/post/rating")]
        public IActionResult Post([FromBody] PostRatingModel model)
        {
            model.UserId = User.GetUserId();
            //model.Rating = 3;

            var factory = new ConnectionFactory() { HostName = "localhost" };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();


            channel.QueueDeclare(queue: "post_rate",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            string msg = JsonConvert.SerializeObject(model);
            var body = Encoding.UTF8.GetBytes(msg);

            channel.BasicPublish(exchange: "",
                 routingKey: "post_rate",
                 basicProperties: null,
                 body: body);

            return Ok();
        }
    }

    public class PostRatingModel
    {
        public string PostId { get; set; }
        public string UserId { get; set; }
        public int Rating { get; set; }

    }
}
