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

        public PostClient(HttpClient client)
        {
            client.BaseAddress = new Uri("http://localhost:5301/api/");
            Client = client;
        }

        public async Task<bool> Create(PostCreateModel post)
        {
            var data = JsonConvert.SerializeObject(post);

            var polly = Policy
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(3, pause => TimeSpan.FromSeconds(5));

            bool success;
            try
            {
                success = await polly.ExecuteAsync(async () =>
                {
                    var response = await Client.PostAsync("post", new StringContent(data, Encoding.UTF8, "application/json"));
                    response.EnsureSuccessStatusCode();
                    return true;
                });
            }
            catch (Exception)
            {

                success = false;
            }
  

            return success;

        }

        public async Task<PostViewModel> Get(string postId)
        {
            var response = await Client.GetAsync("post/" + postId);
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

            var response = await Client.GetAsync("post");
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
            var response = await Client.GetAsync("post/user/" + userId);
            var postList = JsonConvert.DeserializeObject<List<PostViewModel>>
                (await response.Content.ReadAsStringAsync());
            response.EnsureSuccessStatusCode();
            return postList;
        }
    }
}
