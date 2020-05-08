using MediatR;
using Photography.Services.Post.Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Post.Infrastructure.EF.Extensions
{
    public static class MediatorExtension
    {
        public static async Task DispatchDomainEventsAsync(this IMediator mediator, PostContext ctx)
        {
            var domainEntities = ctx.ChangeTracker
                .Entries<Entity>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();

            domainEntities.ToList()
                .ForEach(entity => entity.Entity.ClearDomainEvents());

            // NOTE: as DbContext instance is not thread safe, do not use the Select code:
            // events are published in parallel. Instead, use the foreach code, each publishing is awaited.

            //var tasks = domainEvents
            //    .Select(async (domainEvent) =>
            //    {
            //        await mediator.Publish(domainEvent);
            //    });
            //await Task.WhenAll(tasks);

            foreach (var domainEvent in domainEvents)
                await mediator.Publish(domainEvent);
        }
    }
}
