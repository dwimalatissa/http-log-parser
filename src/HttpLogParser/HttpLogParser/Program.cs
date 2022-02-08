using System.IO;
using HttpLogParser.LogProcessing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HttpLogParser
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = Host
                .CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(BuildConfiguration)
                .ConfigureServices(ConfigureServices)
                .ConfigureLogging(ConfigureLogging)
                .Build();

            var logProcessor = host.Services.GetService<IHttpLogProcessor>();
            var config = host.Services.GetService<IConfiguration>();

            var files = Directory.GetFiles(config["LogPath"]);

            foreach (var file in files)
            {
                logProcessor?.CalculateLogMetrics(file);
            }

            host.Run();
        }

        static void BuildConfiguration(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
        }

        static void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddLogging()
                .AddScoped<IHttpLogProcessor, HttpLogProcessor>();
        }

        static void ConfigureLogging(ILoggingBuilder loggingBuilder)
        {
            loggingBuilder
                .ClearProviders()
                .AddConsole();
        }
    }
}
