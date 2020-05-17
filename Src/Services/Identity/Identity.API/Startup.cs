using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Identity.API.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Photography.Services.Identity.API.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Photography.Services.Identity.API.Application.Commands.LikePost;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Photography.Services.Identity.API.Application.Behaviors;
using Photography.Services.Identity.Infrastructure.EF;
using Arise.DDD.Infrastructure.Data.EF.Extensions;
using System.Reflection;
using AutoMapper;
using Microsoft.OpenApi.Models;
using FluentValidation.AspNetCore;
using Photography.Services.Identity.API.Application.Validators;
using Autofac;
using Photography.Services.Identity.API.Infrastructure.AutofacModules;
using Photography.Services.Identity.API.Query.MapperProfiles;

namespace Photography.Services.Identity.API
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
                    options.Authority = "https://localhost:10001";
                    options.Audience = "Photography.Post.API";
                });

            services.AddHttpContextAccessor();

            services.AddMediatR(typeof(LikePostCommandHandler));

            services.AddSqlDataAccessServices<ApplicationDbContext>(Configuration.GetConnectionString("IdentityConnection"), typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
            services.AddSqlDataAccessServices<IdentityContext>(Configuration.GetConnectionString("IdentityConnection"), typeof(Startup).GetTypeInfo().Assembly.GetName().Name);

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddRazorPages();

            services.AddControllers(options =>
            {
                //options.Filters.Add(typeof(HttpGlobalExceptionFilter));
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

            services.AddAutoMapper(typeof(UserViewModelProfile).Assembly);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Photography.Post API", Version = "v1" });
                c.IncludeXmlComments(string.Format(@"{0}\Post.API.xml", System.AppDomain.CurrentDomain.BaseDirectory));
                c.DescribeAllEnumsAsStrings();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new ApplicationModule());
        }
    }
}
