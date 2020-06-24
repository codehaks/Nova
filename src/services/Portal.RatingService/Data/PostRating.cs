using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portal.RatingService.Data
{
    public class PostRating
    {
        public string PostId { get; set; }
        public string UserId { get; set; }
        public DateTime TimeCreated { get; set; }
        public int Rate { get; set; }
    }
}
