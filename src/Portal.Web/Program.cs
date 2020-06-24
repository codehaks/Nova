using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Portal.Web
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

                    webBuilder.UseSerilog((webHostBuilderContext, logger) =>
                    {
                        if (webHostBuilderContext.HostingEnvironment.IsProduction())
                        {
                            logger.WriteTo.MSSqlServer(
                                webHostBuilderContext.Configuration.GetSection("Logging:mssql").Value,
                                "Logs").MinimumLevel.Error();
                        }
                        else
                        {
                            logger.WriteTo.Console().MinimumLevel.Information();
                        }
                    });
                });
    }
}
