using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Arise.DDD.API.Filters;
using Arise.DDD.API.Response;
using Arise.DDD.Infrastructure.Extensions;
using Autofac;
using AutoMapper;
using FluentValidation.AspNetCore;
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
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Photography.Services.Post.API.Application.Behaviors;
using Photography.Services.Post.API.Application.Commands.Post.PublishPost;
using Photography.Services.Post.API.Application.Commands.User.CreateUser;
using Photography.Services.Post.API.Application.Validators;
using Photography.Services.Post.API.Infrastructure.AutofacModules;
using Photography.Services.Post.API.Query.MapperProfiles;
using Photography.Services.Post.API.Query.ViewModels;
using Photography.Services.Post.API.Settings;
using Photography.Services.Post.Infrastructure;
using Serilog;

namespace Photography.Services.Post.API
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
                    builder.WithOrigins("http://localhost:4200")
                        .WithHeaders(HeaderNames.ContentType, HeaderNames.Authorization, "x-requested-with")
                        .AllowCredentials();
                });
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = Configuration.GetValue<string>("AuthSettings:Authority");
                    options.Audience = Configuration.GetValue<string>("AuthSettings:Audience");
                    options.RequireHttpsMetadata = false;
                });

            services.AddHttpContextAccessor();

            services.AddMediatR(typeof(CreateUserCommandHandler));

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
            //services.AddScoped<IPostQueries>(sp => new PostQueries(Configuration.GetConnectionString("PostConnection")));

            //var dbSettings = new DbSettings();
            //Configuration.GetSection("DbSettings").Bind(dbSettings);
            services.AddSqlDataAccessServices<PostContext>(Configuration.GetConnectionString("PostConnection"), typeof(Startup).GetTypeInfo().Assembly.GetName().Name);

            services.AddAutoMapper(typeof(PostViewModelProfile).Assembly);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Photography.Post API", Version = "v1" });
                c.IncludeXmlComments(string.Format(@"{0}/Post.API.xml", System.AppDomain.CurrentDomain.BaseDirectory));
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

            app.UseCors(_corsPolicy);

            // return a json error when unauthorized
            app.UseStatusCodePages(async context =>
            {
                if (context.HttpContext.Response.StatusCode == 401 ||
                    context.HttpContext.Response.StatusCode == 403)
                {
                    context.HttpContext.Response.ContentType = "application/json";
                    var json = JsonConvert.SerializeObject(ResponseWrapper.CreateErrorResponseWrapper(StatusCode.Unauthorized, "Unauthorized."));
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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Photography.Post API V1");
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
