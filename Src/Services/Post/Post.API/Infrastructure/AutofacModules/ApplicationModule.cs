using Autofac;
using Photography.Services.Post.Domain.Seedwork;
using Photography.Services.Post.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Infrastructure.AutofacModules
{
    public class ApplicationModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // register repositories
            builder.RegisterAssemblyTypes(typeof(PostRepository).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IRepository<>))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}
