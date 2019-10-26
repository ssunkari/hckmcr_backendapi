using System;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Swashbuckle.AspNetCore.Swagger;
namespace Zuto.Uk.Sample.API
{
    public class Program
    {
        private static readonly string Environment = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        public static int Main(string[] args)
        {
            var webHostBuilder = CreateWebHostBuilder(args);

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
                .UseApplicationInsights("f4d84735-e39e-45e8-bf87-c9e24021634f")
                .UseSerilog()
                .UseStartup<Startup>();
    }
}
