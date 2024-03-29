﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Amazon.SQS;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

            services.AddCors(options =>
            {
                options.AddPolicy("*",
                    builder =>
                    {
                        builder.WithOrigins("*").AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });
            awsOptions.Region = RegionEndpoint.EUWest2;
            services.AddDefaultAWSOptions(awsOptions);
            services.AddAWSService<IAmazonDynamoDB>();
            services.AddAWSService<IAmazonSQS>();
            services.AddApplicationInsightsTelemetry();
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
            app.UseSwagger();
            app.UseCors("*");
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
