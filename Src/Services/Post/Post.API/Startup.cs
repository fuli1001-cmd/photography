using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Photography.Services.Post.API.Application.Behaviors;
using Photography.Services.Post.API.Application.Commands;
using Photography.Services.Post.API.Application.Queries;
using Photography.Services.Post.API.Application.Validators;
using Photography.Services.Post.API.Infrastructure.AutofacModules;
using Photography.Services.Post.API.Infrastructure.Filters;
using Photography.Services.Post.Infrastructure.EF.Extensions;

namespace Photography.Services.Post.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMediatR(typeof(PublishPostCommandHandler));
            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(HttpGlobalExceptionFilter));
            })
            .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<PublishPostCommandValidator>());
            services.AddApiVersioning(options =>
            {
                // Specify the default API Version as 1.0
                options.DefaultApiVersion = new ApiVersion(1, 0);
                // If the client hasn't specified the API version in the request, use the default API version number
                options.AssumeDefaultVersionWhenUnspecified = true;
                // Advertise the API versions supported for the particular endpoint
                options.ReportApiVersions = true;
                // default query paramter for version is api-version
                //options.ApiVersionReader = new QueryStringApiVersionReader("v");
            });

            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddScoped<IPostQueries>(sp => new PostQueries(Configuration["ConnectionString"]));

            //var dbSettings = new DbSettings();
            //Configuration.GetSection("DbSettings").Bind(dbSettings);
            services.AddDataAccessServices(Configuration["ConnectionString"], typeof(Startup).GetTypeInfo().Assembly.GetName().Name);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TexasHoldem API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TexasHoldem API V1");
            });
        }

        // ConfigureContainer is where you can register things directly
        // with Autofac. This runs after ConfigureServices so the things
        // here will override registrations made in ConfigureServices.
        // Don't build the container; that gets done for you by the factory.
        public void ConfigureContainer(ContainerBuilder builder)
        {
            //builder.RegisterModule(new MediatorModule());
            builder.RegisterModule(new ApplicationModule());
        }
    }
}
