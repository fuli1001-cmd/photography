using Arise.DDD.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using Photography.Services.Post.Domain.AggregatesModel.UserCommentRelationAggregate;
using Photography.Services.Post.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.DomainEventHandlers.CommentDeleted
{
    public class CommentDeletedDomainEventHandler : INotificationHandler<CommentDeletedDomainEvent>
    {
        private readonly IUserCommentRelationRepository _userCommentRelationRepository;
        private readonly IPostRepository _postRepository;
        private readonly ILogger<CommentDeletedDomainEventHandler> _logger;

        public CommentDeletedDomainEventHandler(
            IUserCommentRelationRepository userCommentRelationRepository,
            IPostRepository postRepository,
            ILogger<CommentDeletedDomainEventHandler> logger)
        {
            _userCommentRelationRepository = userCommentRelationRepository ?? throw new ArgumentNullException(nameof(userCommentRelationRepository));
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(CommentDeletedDomainEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("----- Handling CommentDeletedDomainEvent: at {AppName} - ({@DomainEvent})", Program.AppName, notification);

            // 删除“用户-平论”关系
            var relations = await _userCommentRelationRepository.GetRelationsByCommentIdsAsync(notification.CommentIds);
            relations.ForEach(r => _userCommentRelationRepository.Remove(r));

            // 更新帖子的评论数
            var post = await _postRepository.GetByIdAsync(notification.PostId);
            if (post != null)
                post.DecreaseCommentCount(notification.CommentIds.Count);
        }
    }
}
