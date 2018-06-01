using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Generic_Web_HostBuilder
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = new HostBuilder()
               .ConfigureHostConfiguration(configHost =>
               {
                   configHost.AddEnvironmentVariables(prefix: "ASPNETCORE_");
                   configHost.SetBasePath(Directory.GetCurrentDirectory());
                   configHost.AddJsonFile("hostsettings.json", optional: true);
                   configHost.AddCommandLine(args);
               })
               .ConfigureAppConfiguration((hostContext, configApp) =>
               {
                   configApp.AddEnvironmentVariables(prefix: "ASPNETCORE_");
                   configApp.AddJsonFile("appsettings.json", optional: true);
                   configApp.AddJsonFile(
                       $"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json",
                       optional: true);
                   configApp.AddCommandLine(args);
               })
               .ConfigureServices((hostContext, services) =>
               {
                   services.AddLogging();
                   services.AddHostedService<LifetimeEventsHostedService>();
                   services.AddHostedService<TimedHostedService>();
               })
               .ConfigureLogging((hostContext, configLogging) =>
               {
                   configLogging.AddConsole();
                   configLogging.AddDebug();
               })
               .UseConsoleLifetime()
               .Build();

            await Task.WhenAll(CreateWebHostBuilder(args).Build().RunAsync(), host.RunAsync());
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
