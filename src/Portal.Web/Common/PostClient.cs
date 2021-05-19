using Grpc.Core.Logging;
using Microsoft.AspNetCore.Identity.UI.V3.Pages.Internal.Account.Manage;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        private readonly IDistributedCache _cache;
        private readonly ILogger<PostClient> _logger;

        public PostClient(HttpClient client,IConfiguration configuration, IDistributedCache cache, ILogger<PostClient> logger)
        {
            var postService = configuration.GetServiceUri("portal-postservice");
            client.BaseAddress = postService;// new Uri(postService+ "/api/");
            Client = client;
            Configuration = configuration;
            _cache = cache;
            _logger = logger;
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
            string data;

            
            var cachedPosts = _cache.Get("posts");
            if (cachedPosts==null)
            {
                data= await response.Content.ReadAsStringAsync();
                _cache.Set("posts", Encoding.UTF8.GetBytes(data), new DistributedCacheEntryOptions
                {
                   AbsoluteExpirationRelativeToNow=TimeSpan.FromMinutes(10)
                });
                _logger.LogInformation("Data cached to redis");
                
            }
            else
            {
                _logger.LogInformation("Data loaded from redis cache");
                data = Encoding.UTF8.GetString(cachedPosts);
            }

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
            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty( content)==false)
            {
                var postList = JsonConvert.DeserializeObject<List<PostViewModel>>
                               (content);
                response.EnsureSuccessStatusCode();
                return postList;
            }

            return null;
           
        }
    }
}
