using System;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Swashbuckle.AspNetCore.Swagger;
using Zuto.Uk.Infrastructure.Logging;
using Zuto.Uk.Infrastructure.Logging.Enrichers;

namespace Zuto.Uk.Sample.API
{
    public class Program
    {
        private static readonly string Environment = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        public static int Main(string[] args)
        {
            var webHostBuilder = CreateWebHostBuilder(args);

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .Build();

            var loggerConfiguration = new LoggerConfiguration()
                //Need below to disable default microsoft logging
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                //Load additional config from appsettings
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                //.Filter.ByExcluding("RequestMethod = 'GET' and RequestPath = '/api/health'")
                .Enrich.With(new HostIpEnricher())
                .Enrich.WithProperty("type", "sample-web-api")
                .Enrich.WithProperty("environment", Environment)
                 //.WriteTo.Console(new MessageTemplateTextFormatter(
                 //    "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}", null
                 //))
                .WriteTo
                .ApplicationInsightsEvents("f4d84735-e39e-45e8-bf87-c9e24021634f")
                .WriteTo.Udp("192.168.50.2", 8901, new LogstashJsonFormatter(), 0, LogEventLevel.Information);

            Log.Logger = loggerConfiguration.CreateLogger();
            //In case of errors within Serilog, it is possible to debug logs from Serilog using SelfLog.
            Serilog.Debugging.SelfLog.Enable(Console.Error);

            try
            {
                Log.Information("Starting Web host");
                webHostBuilder.Build().Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureServices(s=>
                {
                    s.AddAutofac();
                    s.AddSwaggerGen(c => { c.SwaggerDoc("v1", new Info {Title = "Sample API", Version = "v1"}); });
                })
                .UseIISIntegration()
                .UseSerilog()
                .UseStartup<Startup>();
    }
}
