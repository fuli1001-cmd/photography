using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Threading.Tasks;

namespace ConsoleClient
{
    class Program
    {
        public static readonly string AppName = typeof(Program).Namespace;

        static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureLogging((hostBuilderContext, loggingBuilder) =>
            {
                Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();
            })
            .UseSerilog()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHttpClient<PostService>();
                services.AddHostedService<App>();
            });
    }
}
