using Arise.DDD.Domain.SeedWork;
using Autofac;
using Photography.Services.Identity.API.Query.EF;
using Photography.Services.Identity.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Photography.Services.Identity.API.Infrastructure.AutofacModules
{
    public class ApplicationModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // register repositories
            builder.RegisterAssemblyTypes(typeof(UserRepository).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IRepository<>))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            // register queries
            builder.RegisterAssemblyTypes(typeof(UserQueries).GetTypeInfo().Assembly)
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}
