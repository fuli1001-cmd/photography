using MediatR;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using Photography.Services.Post.Domain.Events;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.DomainEventHandlers.RepliedPost
{
    public class RepliedPostDomainEventHandler : INotificationHandler<RepliedPostDomainEvent>
    {
        private readonly IPostRepository _postRepository;
        private readonly ILogger<RepliedPostDomainEventHandler> _logger;

        public RepliedPostDomainEventHandler(
            IPostRepository postRepository,
            ILogger<RepliedPostDomainEventHandler> logger)
        {
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(RepliedPostDomainEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("----- Handling RepliedPostDomainEvent: at {AppName} - ({@DomainEvent})", Program.AppName, notification);

            var post = await _postRepository.GetByIdAsync(notification.PostId);
            post.Comment();
        }
    }
}
