using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Photography.Services.Post.API.Settings;
using Photography.Services.Post.Domain.AggregatesModel.CommentAggregate;
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
        private readonly ICommentRepository _commentRepository;
        private readonly PostScoreRewardSettings _scoreRewardSettings;
        private readonly ILogger<UserUnLikedCommentDomainEventHandler> _logger;

        public UserUnLikedCommentDomainEventHandler(
            IPostRepository postRepository,
            ICommentRepository commentRepository,
            IOptionsSnapshot<PostScoreRewardSettings> scoreRewardOptions,
            ILogger<UserUnLikedCommentDomainEventHandler> logger)
        {
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _commentRepository = commentRepository ?? throw new ArgumentNullException(nameof(commentRepository));
            _scoreRewardSettings = scoreRewardOptions?.Value ?? throw new ArgumentNullException(nameof(scoreRewardOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(UserUnLikedCommentDomainEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("----- Handling UserUnLikedCommentDomainEvent: at {AppName} - ({@DomainEvent})", Program.AppName, notification);

            var post = await _postRepository.GetByIdAsync(notification.PostId);
            post.UnLike(_scoreRewardSettings.LikePost);

            var comment = await _commentRepository.GetByIdAsync(notification.CommentId);
            comment.UnLike();
        }
    }
}
