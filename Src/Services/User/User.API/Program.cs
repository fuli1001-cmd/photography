using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Serilog;

namespace Photography.Services.User.API
{
    public class Program
    {
        public static readonly string Namespace = typeof(Program).Namespace;
        public static readonly string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);

        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            try
            {
                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Program terminated unexpectedly ({ApplicationContext})!", AppName);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseNServiceBus(hostBuilderContext =>
                {
                    var endpointConfiguration = new EndpointConfiguration("userapi");
                    //var transport = endpointConfiguration.UseTransport<LearningTransport>();
                    var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
                    transport.ConnectionString(hostBuilderContext.Configuration.GetConnectionString("RabbitMQ"));
                    transport.UseConventionalRoutingTopology();
                    endpointConfiguration.EnableInstallers();

                    //endpointConfiguration.SendFailedMessagesTo("error");
                    //endpointConfiguration.AuditProcessedMessagesTo("audit");
                    //endpointConfiguration.SendHeartbeatTo("Particular.ServiceControl");
                    //var metrics = endpointConfiguration.EnableMetrics();
                    //metrics.SendMetricDataToServiceControl("Particular.Monitoring", TimeSpan.FromMilliseconds(500));

                    return endpointConfiguration;
                })
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureLogging((hostBuilderContext, loggingBuilder) =>
                {
                    Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .Enrich.WithProperty("ApplicationContext", AppName)
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs/.txt"), rollingInterval: RollingInterval.Day)
                    //.WriteTo.Seq("http://seq")
                    //.ReadFrom.Configuration(hostBuilderContext.Configuration)
                    .CreateLogger();
                })
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
