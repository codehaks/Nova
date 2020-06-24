using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portal.PostService.Models
{
    public class Post
    {
        public Guid Id { get; set; }

        public int CityId { get; set; }
        public int CategoryId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public DateTime TimeCreated { get; set; }

        public string UserId { get; set; }

        public PostState State { get; set; }
    }

    public enum PostState
    {
        Active = 0,
        Banned = 1,
        Archived = 2,

    }
}
