using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace Portal.Web.Pages.Posts
{
    public class DetailsModel : PageModel
    {
        public async Task<IActionResult> OnGet(string id)
        {
            var client = new HttpClient();

            var response = await client.GetAsync("http://localhost:5301/api/post/" + id);
            Post = JsonConvert.DeserializeObject<PostViewModel>
                (await response.Content.ReadAsStringAsync());
            response.EnsureSuccessStatusCode();
            return Page();
        }

   

        [BindProperty]
        public PostViewModel Post { get; set; }

        public class PostViewModel
        {
            public string Id { get; set; }
            public string Name { get; set; }

            public string Description { get; set; }
            public Byte[] ImageData { get; set; }
        }
    }
}