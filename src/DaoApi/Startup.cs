using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using CorrelationId;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serilog;
using Serilog.Context;
using Zuto.Uk.Sample.API.Ioc;

namespace Zuto.Uk.Sample.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            var awsOptions = Configuration.GetAWSOptions();
            awsOptions.Credentials = new BasicAWSCredentials("AKIA324BBMP6VDMKFK6Z", "cEFCHpDseed6stn/FgQAQkOJNv1IW7j/m6UDfOZm")
            {
                
            };
            awsOptions.Region = RegionEndpoint.EUWest2;
            services.AddDefaultAWSOptions(awsOptions);
            services.AddAWSService<IAmazonDynamoDB>();
            services.AddCorrelationId();
            services.AddApplicationInsightsTelemetry();
            services.TryAddScoped<ICorrelationContextAccessor, CorrelationContextAccessor>();
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<ServiceModule>();
            containerBuilder.Populate(services);
            var container = containerBuilder.Build();
            return new AutofacServiceProvider(container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            app.UseCorrelationId(new CorrelationIdOptions
            {
                UseGuidForCorrelationId = true,
                Header = "x-correlation-id",
                IncludeInResponse = true
            });
           
            app.Use(async (ctx, next) =>
            {
                using (LogContext.Push(new CorrelationIdEnricher(ctx.RequestServices.GetService<ICorrelationContextAccessor>())))
                {
                    await next();
                }
            });

            app.Use(async (ctx, next) =>
            {
                var sw = new Stopwatch();
                sw.Start();
                var correlationId = ctx.RequestServices.GetService<ICorrelationContextAccessor>().CorrelationContext
                    .CorrelationId;
                ctx.Response.OnCompleted(() =>
                {
                    sw.Stop();
                    var logger = Log.ForContext("SourceContext", "HttpContextInfo");
                    using (LogContext.PushProperty("CorrelationId", correlationId))
                    {
                        logger.Information(
                            "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {TotalTimeInMs:0.0000} ms",
                            ctx.Request.Method, ctx.Request.Path, ctx.Response?.StatusCode, sw.Elapsed.TotalMilliseconds);
                    }

                    return Task.CompletedTask;
                });
                await next.Invoke();
            });

            //Add Swagger middleware
            //https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-2.2&tabs=visual-studio
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty;
            });

            //Redirect to Swagger https://github.com/domaindrivendev/Swashbuckle/issues/1227
            var option = new RewriteOptions();
            option.AddRedirect("^$", "swagger");
            app.UseRewriter(option);
            app.UseMvc();
        }
    }
}
