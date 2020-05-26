using MediatR;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using Photography.Services.Post.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.DomainEventHandlers.UserUnLikedPost
{
    public class UserUnLikedPostDomainEventHandler : INotificationHandler<UserUnLikedPostDomainEvent>
    {
        private readonly IPostRepository _postRepository;
        private readonly ILogger<UserUnLikedPostDomainEventHandler> _logger;

        public UserUnLikedPostDomainEventHandler(
            IPostRepository postRepository,
            ILogger<UserUnLikedPostDomainEventHandler> logger)
        {
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(UserUnLikedPostDomainEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("----- Handling UserUnLikedPostDomainEvent: at {AppName} - ({@DomainEvent})", Program.AppName, notification);

            var post = await _postRepository.GetByIdAsync(notification.PostId);
            post.UnLike();
            await _postRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
