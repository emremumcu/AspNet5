using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/// PM> Install-Package NLog.Web.AspNetCore 

namespace AspNet5
{    
    public class Program
    {
        public static void Main(string[] args)
        {
            /// IServiceScope scope = host.Services.CreateScope();
            /// IServiceProvider services = scope.ServiceProvider;
            /// IWebHostEnvironment environment = services.GetRequiredService<IWebHostEnvironment>();

            IConfigurationRoot configRoot = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddEnvironmentVariables()
                .AddJsonFile("appsettings.json", optional: false)
                .AddCommandLine(args)
                .Build();

            Logger logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

            try
            {
#if DEBUG
                IHost host = CreateHostBuilderLocal(args).Build();
                host.Run();
#else
                IHost host = CreateHostBuilder(args).Build();
                host.Run();                
#endif  
            }
            catch (Exception exception)
            {
                logger.Error(exception, "Main");
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }

        /// <summary>
        /// CreateHostBuilder (for IIS / IIS Express)
        /// </summary>
        public static IHostBuilder CreateHostBuilderLocal(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    /// HACK Change WebRoot and ContentRoot if required
                    /// webBuilder.UseContentRoot(System.IO.Directory.GetCurrentDirectory());
                    /// webBuilder.UseWebRoot("wwwroot");
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                })
                .UseNLog();

        /// <summary>
        /// CreateHostBuilder (for Kestrel)
        /// </summary>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    /// HACK Change WebRoot and ContentRoot if required
                    /// webBuilder.UseContentRoot(System.IO.Directory.GetCurrentDirectory());
                    /// webBuilder.UseWebRoot("wwwroot");
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseKestrel();
                    /// HACK Change Kestrel Service Port
                    webBuilder.UseUrls("http://localhost:5000");
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                })
                .UseNLog();
    }
}
