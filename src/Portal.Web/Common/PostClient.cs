using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Polly;
using Portal.Web.Areas.User.Pages.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Web.Common
{
    public class PostClient
    {
        public HttpClient Client { get; }
        public IConfiguration Configuration { get; }

        public PostClient(HttpClient client,IConfiguration configuration)
        {
            var postService = configuration.GetServiceUri("portal-postservice");
            client.BaseAddress = postService;// new Uri(postService+ "/api/");
            Client = client;
            Configuration = configuration;
        }

        public async Task<bool> Create(PostCreateModel post)
        {
            var data = JsonConvert.SerializeObject(post);

            var polly = Policy
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(3, pause => TimeSpan.FromSeconds(5));


            await polly.ExecuteAsync(async () =>
            {
                var response = await Client.PostAsync("api/post", new StringContent(data, Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();
                return true;
            });

            return false;

        }

        public async Task<PostViewModel> Get(string postId)
        {
            var response = await Client.GetAsync("api/post/" + postId);
            var post = JsonConvert.DeserializeObject<PostViewModel>
                (await response.Content.ReadAsStringAsync());
            response.EnsureSuccessStatusCode();
            return post;
        }

        public async Task<bool> Update(PostViewModel post)
        {
            var req = new HttpRequestMessage(HttpMethod.Put, "post");
            var data = JsonConvert.SerializeObject(post);
            req.Content = new StringContent(data, Encoding.UTF8, "application/json");
            var response = await Client.SendAsync(req);
            response.EnsureSuccessStatusCode();
            return true;
        }

        public async Task<List<PostViewModel>> GetAll()
        {

            var response = await Client.GetAsync("api/post");
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();

            List<PostViewModel> PostList;
            if (!string.IsNullOrEmpty(data))
            {
                PostList = JsonConvert.DeserializeObject<List<PostViewModel>>(data);
            }
            else
            {
                PostList = new List<PostViewModel>();
            }

            return PostList;
        }

        public async Task<List<PostViewModel>> GetAllByUserId(string userId)
        {
            var response = await Client.GetAsync("api/post/user/" + userId);
            var postList = JsonConvert.DeserializeObject<List<PostViewModel>>
                (await response.Content.ReadAsStringAsync());
            response.EnsureSuccessStatusCode();
            return postList;
        }
    }
}
