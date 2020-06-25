using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Design;
using Newtonsoft.Json;
using Polly;
using Portal.ImageService.Protos;
using Portal.Web.Common;

namespace Portal.Web.Areas.User.Pages.Posts
{
    public class CreateModel : PageModel
    {
        private readonly PostClient _postClient;

        public CreateModel(PostClient postClient)
        {
            _postClient = postClient;
        }

        public async Task<IActionResult> OnPost()
        {
            Post.Id = Guid.NewGuid();
            Post.UserId = User.GetUserId();

            var polly = Polly.Policy.Handle<Exception>()
                .CircuitBreakerAsync(2,TimeSpan.FromSeconds(20));

            await polly.ExecuteAsync(async () => {
                using var channel = GrpcChannel.ForAddress("https://localhost:5303");
                var uploadFileClient = new UploadFileService.UploadFileServiceClient(channel);
               await SendFile(uploadFileClient, Post.File, Post.Id.ToString());
            });

       

            
            var result=await _postClient.Create(Post);

            if (result==false)
            {
                return Page();
            }

            return RedirectToPage("./index");
        }

        [BindProperty]
        public PostCreateModel Post { get; set; }

        private static async Task SendFile(UploadFileService.UploadFileServiceClient client, IFormFile filePath, string postId)
        {
            byte[] buffer;
            var fileStream = new MemoryStream();
            await filePath.CopyToAsync(fileStream);
            fileStream.Position = 0;
            try
            {
                int length = (int)fileStream.Length;
                buffer = new byte[length];
                int count;
                int sum = 0;

                while ((count = await fileStream.ReadAsync(buffer, sum, length - sum)) > 0)
                    sum += count;
            }
            finally
            {
                fileStream.Close();
            }

            var result = await client.SendFileAsync(new Chunk
            {
                PostId = postId,
                Content = ByteString.CopyFrom(buffer)
            });

            Console.WriteLine(result.Success);

        }

    }
}
