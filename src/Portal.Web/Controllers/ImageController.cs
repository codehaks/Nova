using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Portal.Web.Controllers
{
    public class ImageController : Controller
    {
        private readonly IConfiguration _configuration;

        public ImageController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Route("api/image/{id}")]
        public async Task<IActionResult> GetImage(string id)
        {
            var postImage = _configuration.GetServiceUri("portal-imagedeliveryservice");
            var client = new HttpClient();
            client.BaseAddress = postImage;
            var result=await client.GetAsync("api/image/thumb/" + id);
            return File(await result.Content.ReadAsByteArrayAsync(), "image/jpeg");
        }
    }
}
