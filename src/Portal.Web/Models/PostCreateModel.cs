using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Portal.Web.Areas.User.Pages.Posts
{


    public class PostCreateModel
    {
        public Guid Id { get; set; }

        [Required]
        public int CityId { get; set; }
        [Required]
        public int CategoryId { get; set; }

        [MaxLength(100)]
        [Required]
        public string Name { get; set; }

        [MaxLength(256)]
        [Required]
        public string Description { get; set; }

        public string UserId { get; set; }

        public IFormFile File { get; set; }
    }

}
