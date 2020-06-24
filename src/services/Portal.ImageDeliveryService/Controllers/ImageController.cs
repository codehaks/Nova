using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Portal.ImageDeliveryService.Data;

namespace Portal.ImageDeliveryService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly ImageDbContext _db;

        public ImageController(ImageDbContext db)
        {
            _db = db;
        }

        [Route("full/{id}")]
        public async Task<IActionResult> GetImage(string id)
        {
            var image=await _db.Images.FindAsync(Guid.Parse(id));
            return File(image.Full, "image/jpeg");
        }

        [Route("thumb/{id}")]
        public async Task<IActionResult> GetImageThumb(string id)
        {
            var image = await _db.Images.FindAsync(Guid.Parse(id));
            return File(image.Thumb, "image/jpeg");
        }
    }
}
