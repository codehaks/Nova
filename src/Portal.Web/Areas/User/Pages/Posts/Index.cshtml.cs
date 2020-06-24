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
    public class IndexModel : PageModel
    {
        private readonly PostClient _postClient;

        public IndexModel(PostClient postClient)
        {
            _postClient = postClient;
        }

        public async Task<IActionResult> OnGet()
        {
                var userId = User.GetUserId();

            PostList = await _postClient.GetAllByUserId(userId);
            return Page();
        }

        public List<PostViewModel> PostList { get; set; }
    }
}
