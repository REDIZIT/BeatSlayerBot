using BeatSlayerServer.Models.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace BeatSlayerBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var config = new ConfigurationBuilder()
                .AddJsonFile($"Configs/serversettings.json")
                .Build();

            ServerSettings settings = new ServerSettings();
            config.Bind(settings);

            CreateHostBuilder(args, settings.Host.StartUrl).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args, string startUrl) =>
            Host.CreateDefaultBuilder(args)
            .UseSystemd()
            .ConfigureAppConfiguration(builder =>
            {
                builder.AddJsonFile("Configs/serversettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder
                    .UseUrls(startUrl)
                    .UseStartup<Startup>();
            });
    }
}
