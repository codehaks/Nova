using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.MSSqlServer;

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
                        logger.WriteTo.Console().MinimumLevel.Information();

                        if (webHostBuilderContext.HostingEnvironment.IsDevelopment())
                        {
              
                            var columnOpts = new ColumnOptions();
                            columnOpts.Store.Remove(StandardColumn.Properties);
                            columnOpts.Store.Add(StandardColumn.LogEvent);
                            columnOpts.LogEvent.DataLength = 2048;
                            columnOpts.PrimaryKey = columnOpts.Id;
                            columnOpts.Id.DataType = System.Data.SqlDbType.Int;
                         

                            logger.WriteTo
                            .MSSqlServer(webHostBuilderContext.Configuration
                            .GetSection("Logging:mssql").Value, "Logs",autoCreateSqlTable:true)
                            .MinimumLevel.Information();
                        }
                        else
                        {
                            logger.WriteTo.Console().MinimumLevel.Information();
                        }
                    });
                });
    }
}
