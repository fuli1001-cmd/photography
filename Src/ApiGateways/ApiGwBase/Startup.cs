using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Photography.ApiGateways.ApiGwBase.Redis;
using Photography.ApiGateways.ApiGwBase.Services;
using Photography.ApiGateways.ApiGwBase.Settings;
using Photography.ApiGateways.ApiGwBase.Sms;

namespace Photography.ApiGateways.ApiGwBase
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //    .AddJwtBearer("Bearer", options =>
            //    {
            //        options.Authority = Configuration["AuthSettings:Authority"];
            //        options.Audience = Configuration["AuthSettings:Audience"];
            //        options.RequireHttpsMetadata = false;
            //    });

            //services.AddOcelot().AddConsul().AddConfigStoredInConsul();
            services.AddOcelot(Configuration);

            services.Configure<ServerSettings>(Configuration.GetSection("ServerSettings"));
            services.Configure<ServiceSettings>(Configuration.GetSection("ServiceSettings"));
            services.Configure<RedisSettings>(Configuration.GetSection("RedisSettings"));
            services.Configure<SmsSettings>(Configuration.GetSection("SmsSettings"));

            services.AddTransient(typeof(IRedisService), typeof(RedisService));
            services.AddTransient(typeof(ISmsService), typeof(AliSmsService));

            services.AddControllers();

            services.AddHttpClient<AuthService>();
            services.AddHttpClient<UserService>();
            services.AddHttpClient<NotificationService>();

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

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Photography.ApiGateway API", Version = "v1" });
                c.IncludeXmlComments(string.Format(@"{0}/ApiGwBase.xml", System.AppDomain.CurrentDomain.BaseDirectory));
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

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                //endpoints.MapGet("/", async context =>
                //{
                //    await context.Response.WriteAsync("Hello World!");
                //});
            });

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Photography.ApiGateway API V1");
            });

            app.UseOcelot().Wait();
        }
    }
}
