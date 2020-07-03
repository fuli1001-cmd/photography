using MediatR;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using Photography.Services.Post.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.DomainEventHandlers.PostShared
{
    public class PostSharedDomainEventHandler : INotificationHandler<PostSharedDomainEvent>
    {
        private readonly IPostRepository _postRepository;
        private readonly ILogger<PostSharedDomainEventHandler> _logger;

        public PostSharedDomainEventHandler(
            IPostRepository postRepository,
            ILogger<PostSharedDomainEventHandler> logger)
        {
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(PostSharedDomainEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("----- Handling PostSharedDomainEvent: at {AppName} - ({@DomainEvent})", Program.AppName, notification);

            var post = await _postRepository.GetByIdAsync(notification.PostId);
            post.Share();
        }
    }
}
