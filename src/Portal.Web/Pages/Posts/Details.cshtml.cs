using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Portal.Web.Areas.User.Pages.Posts;
using Portal.Web.Common;

namespace Portal.Web.Pages.Posts
{
    public class DetailsModel : PageModel
    {
        private readonly PostClient _postClient;

        public DetailsModel(PostClient postClient)
        {
            _postClient = postClient;
        }

        public async Task<IActionResult> OnGet(string id)
        {
            Post = await _postClient.Get(id);         
            return Page();
        }   

        [BindProperty]
        public PostViewModel Post { get; set; }

      
    }
}