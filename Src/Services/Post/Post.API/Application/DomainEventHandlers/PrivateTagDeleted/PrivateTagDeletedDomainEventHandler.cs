using MediatR;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using Photography.Services.Post.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.DomainEventHandlers.PrivateTagDeleted
{
    public class PrivateTagDeletedDomainEventHandler : INotificationHandler<PrivateTagDeletedDomainEvent>
    {
        private readonly IPostRepository _postRepository;
        private readonly ILogger<PrivateTagDeletedDomainEventHandler> _logger;

        public PrivateTagDeletedDomainEventHandler(
            IPostRepository postRepository,
            ILogger<PrivateTagDeletedDomainEventHandler> logger)
        {
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(PrivateTagDeletedDomainEvent notification, CancellationToken cancellationToken)
        {
            var posts = await _postRepository.GetUserPostsByPrivateTag(notification.UserId, notification.Name);
            posts.ForEach(p => p.RemovePrivateTag());
        }
    }
}
