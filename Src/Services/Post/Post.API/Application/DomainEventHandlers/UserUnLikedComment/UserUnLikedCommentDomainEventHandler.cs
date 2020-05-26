using MediatR;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using Photography.Services.Post.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.DomainEventHandlers.UserUnLikedComment
{
    public class UserUnLikedCommentDomainEventHandler : INotificationHandler<UserUnLikedCommentDomainEvent>
    {
        private readonly IPostRepository _postRepository;
        private readonly ILogger<UserUnLikedCommentDomainEventHandler> _logger;

        public UserUnLikedCommentDomainEventHandler(
            IPostRepository postRepository,
            ILogger<UserUnLikedCommentDomainEventHandler> logger)
        {
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(UserUnLikedCommentDomainEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("----- Handling UserUnLikedCommentDomainEvent: at {AppName} - ({@DomainEvent})", Program.AppName, notification);

            var post = await _postRepository.GetByIdAsync(notification.PostId);
            post.UnLike();
            await _postRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
