using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.MSSqlServer.Sinks.MSSqlServer.Options;

namespace Portal.PostService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseSerilog((builder, logger) =>
                    {
                        if (builder.HostingEnvironment.IsDevelopment())
                        {
                            logger.WriteTo.Console().MinimumLevel.Error();
                        }
                        else
                        {
                            logger.WriteTo
                            .MSSqlServer("Data Source=localhost;Initial Catalog=PostServiceLogsDb;Integrated Security=True", new SinkOptions
                            {
                                AutoCreateSqlTable = true,
                                TableName = "Logs"
                            })
                            .MinimumLevel.Error();
                        }
                    });

                });
    }
}
