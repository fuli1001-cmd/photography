using MediatR;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using Photography.Services.Post.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.DomainEventHandlers.UserLikedPost
{
    public class UserLikedPostDomainEventHandler : INotificationHandler<UserLikedPostDomainEvent>
    {
        private readonly IPostRepository _postRepository;
        private readonly ILogger<UserLikedPostDomainEventHandler> _logger;

        public UserLikedPostDomainEventHandler(
            IPostRepository postRepository,
            ILogger<UserLikedPostDomainEventHandler> logger)
        {
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(UserLikedPostDomainEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("----- Handling UserLikedPostDomainEvent: at {AppName} - ({@DomainEvent})", Program.AppName, notification);

            var post = await _postRepository.GetByIdAsync(notification.PostId);
            post.Like();
        }
    }
}
