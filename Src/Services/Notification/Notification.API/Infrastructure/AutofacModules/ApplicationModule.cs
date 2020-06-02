using Arise.DDD.Domain.SeedWork;
using Autofac;
using Photography.Services.Notification.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Photography.Services.Notification.API.Infrastructure.AutofacModules
{
    public class ApplicationModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // register repositories
            builder.RegisterAssemblyTypes(typeof(EventRepository).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IRepository<>))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            //// register queries
            //builder.RegisterAssemblyTypes(typeof(PostQueries).GetTypeInfo().Assembly)
            //    .AsImplementedInterfaces()
            //    .InstancePerLifetimeScope();
        }
    }
}
