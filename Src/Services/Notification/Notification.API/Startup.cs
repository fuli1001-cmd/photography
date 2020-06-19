using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Arise.DDD.API.Filters;
using Arise.DDD.API.Response;
using Arise.DDD.Infrastructure.Extensions;
using Autofac;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Photography.Services.Notification.API.Application.Behaviors;
using Photography.Services.Notification.API.Application.Commands.CreateEvent;
using Photography.Services.Notification.API.Infrastructure.AutofacModules;
using Photography.Services.Notification.Infrastructure;

namespace Photography.Services.Notification.API
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
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = Configuration.GetValue<string>("AuthSettings:Authority");
                    options.Audience = Configuration.GetValue<string>("AuthSettings:Audience");
                    options.RequireHttpsMetadata = false;
                });

            services.AddHttpContextAccessor();

            services.AddMediatR(typeof(CreateEventCommandHandler));

            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(HttpGlobalExceptionFilter));
            });

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

            services.AddSqlDataAccessServices<NotificationContext>(Configuration.GetConnectionString("NotificationConnection"), typeof(Startup).GetTypeInfo().Assembly.GetName().Name);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Photography.Notification API", Version = "v1" });
                c.IncludeXmlComments(string.Format(@"{0}/Notification.API.xml", System.AppDomain.CurrentDomain.BaseDirectory));
                c.DescribeAllEnumsAsStrings();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            IdentityModelEventSource.ShowPII = true;

            //app.UseHttpsRedirection();

            app.UseRouting();

            // return a json error when unauthorized
            app.UseStatusCodePages(async context =>
            {
                if (context.HttpContext.Request.Path.StartsWithSegments("/api") &&
                   (context.HttpContext.Response.StatusCode == 401 ||
                    context.HttpContext.Response.StatusCode == 403))
                {
                    context.HttpContext.Response.ContentType = "application/json";
                    var json = JsonConvert.SerializeObject(ResponseWrapper.CreateErrorResponseWrapper(StatusCode.Unauthorized, "Unauthorized"), new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    });
                    await context.HttpContext.Response.WriteAsync(json);
                }
            });

            app.UseAuthentication();
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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Photography.Notification API V1");
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
