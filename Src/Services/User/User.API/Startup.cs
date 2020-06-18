using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Photography.Services.User.API.Application.Behaviors;
using System.Reflection;
using AutoMapper;
using Microsoft.OpenApi.Models;
using FluentValidation.AspNetCore;
using Photography.Services.User.API.Application.Validators;
using Autofac;
using Photography.Services.User.API.Infrastructure.AutofacModules;
using Photography.Services.User.API.Query.MapperProfiles;
using Photography.Services.User.Infrastructure;
using Arise.DDD.Infrastructure.Extensions;
using Arise.DDD.API.Filters;
using Photography.Services.User.API.Settings;
using Photography.Services.User.API.Application.Commands.Login;
using Photography.Services.User.API.Infrastructure.Redis;
using Photography.Services.User.API.BackwardCompatibility.ChatServerRedis;
using Newtonsoft.Json;
using Arise.DDD.API.Response;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Serialization;
using Microsoft.Net.Http.Headers;

namespace Photography.Services.User.API
{
    public class Startup
    {
        private readonly string _corsPolicy = "CorsPolicy";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(_corsPolicy,
                builder =>
                {
                    builder.WithOrigins(Configuration.GetValue<string>("CorsOrigins").Split(" "))
                        .WithHeaders(HeaderNames.ContentType, HeaderNames.Authorization, "x-requested-with")
                        .AllowCredentials();
                });
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = Configuration["AuthSettings:Authority"];
                    options.Audience = Configuration["AuthSettings:Audience"];
                    options.RequireHttpsMetadata = false;
                });

            services.AddHttpContextAccessor();

            services.AddMediatR(typeof(LoginCommand).GetTypeInfo().Assembly);

            services.Configure<AuthSettings>(Configuration.GetSection("AuthSettings"));
            services.Configure<RedisSettings>(Configuration.GetSection("RedisSettings"));
            services.Configure<ServerSettings>(Configuration.GetSection("ServerSettings"));

            services.AddSingleton(typeof(IRedisService), typeof(RedisService));
            services.AddTransient(typeof(IChatServerRedis), typeof(ChatServerRedis));

            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(HttpGlobalExceptionFilter));
            })
            .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<FollowCommandValidator>());

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

            services.AddSqlDataAccessServices<UserContext>(Configuration.GetConnectionString("UserConnection"), typeof(Startup).GetTypeInfo().Assembly.GetName().Name);

            services.AddAutoMapper(typeof(UserViewModelProfile).Assembly);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Photography.User API", Version = "v1" });
                c.IncludeXmlComments(string.Format(@"{0}/User.API.xml", System.AppDomain.CurrentDomain.BaseDirectory));
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

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(_corsPolicy);

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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Photography.User API V1");
            });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new ApplicationModule());
        }
    }
}
