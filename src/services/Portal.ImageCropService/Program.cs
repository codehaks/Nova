using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Portal.ImageCropService.Data;

namespace Portal.ImageCropService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddScoped<IScopedProcessingService, ScopedProcessingService>();
                    services.AddDbContext<ImageDbContext>(options =>
                options.UseSqlServer("Data Source=localhost;Initial Catalog=Nova_ImageDb;Integrated Security=True"));
                });
    }
}
