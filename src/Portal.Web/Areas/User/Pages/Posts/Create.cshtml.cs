using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core.Logging;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Portal.ImageService.Protos;
using Portal.Web.Common;

namespace Portal.Web.Areas.User.Pages.Posts
{
    public class CreateModel : PageModel
    {
        private readonly PostClient _postClient;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(PostClient postClient, ILogger<CreateModel> logger)
        {
            _postClient = postClient;
            _logger = logger;
        }

        public async Task<IActionResult> OnPost()
        {
            Post.Id = Guid.NewGuid();
            Post.UserId = User.GetUserId();


            var polly = Polly.Policy.Handle<Exception>()
                .WaitAndRetryAsync(2, sleep =>
                {
                    _logger.LogWarning($"ImageService[gRPC] connect retrying... [{sleep}]");
                    return TimeSpan.FromSeconds(5);
                });

            SendResult fileSendResult;

            try
            {
                fileSendResult = await polly.ExecuteAsync(async () =>
               {
                   using var channel = GrpcChannel.ForAddress("https://localhost:5303");
                   var uploadFileClient = new UploadFileService.UploadFileServiceClient(channel);
                   var sendResult = await SendFile(uploadFileClient, Post.File, Post.Id.ToString());
                   return sendResult;
               });
            }
            catch (Exception ex)
            {

                ViewData["Error"] = "Can not send file";
                _logger.LogCritical(ex.Message);
                return Page();
            }


            if (fileSendResult.Success)
            {
                var result = await _postClient.Create(Post);

                if (result == false)
                {
                    return Page();
                }
            }
            else
            {
                ViewData["Error"] = "Can not send file";
                return Page();
            }


            return RedirectToPage("./index");
        }

        [BindProperty]
        public PostCreateModel Post { get; set; }

        private static async Task<SendResult> SendFile(UploadFileService.UploadFileServiceClient client, IFormFile filePath, string postId)
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

            return result;

        }

    }
}
