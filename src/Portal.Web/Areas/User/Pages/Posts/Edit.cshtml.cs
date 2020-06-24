using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Portal.Web.Common;

namespace Portal.Web.Areas.User.Pages.Posts
{
    public class EditModel : PageModel
    {
        private readonly PostClient _postClient;

        public EditModel(PostClient postClient)
        {
            _postClient = postClient;
        }

        public async Task<IActionResult> OnGet(string id)
        {
            Post=await _postClient.Get(id);
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            await _postClient.Update(Post);
            return RedirectToPage("/index");

        }

        [BindProperty]
        public PostViewModel Post { get; set; }

        
    }
}
