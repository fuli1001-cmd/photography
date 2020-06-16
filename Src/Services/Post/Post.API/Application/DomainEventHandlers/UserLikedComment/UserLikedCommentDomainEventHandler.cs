﻿using MediatR;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.Domain.AggregatesModel.CommentAggregate;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using Photography.Services.Post.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.DomainEventHandlers.UserLikedComment
{
    public class UserLikedCommentDomainEventHandler : INotificationHandler<UserLikedCommentDomainEvent>
    {
        private readonly IPostRepository _postRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly ILogger<UserLikedCommentDomainEventHandler> _logger;

        public UserLikedCommentDomainEventHandler(
            IPostRepository postRepository,
            ICommentRepository commentRepository,
            ILogger<UserLikedCommentDomainEventHandler> logger)
        {
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _commentRepository = commentRepository ?? throw new ArgumentNullException(nameof(commentRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(UserLikedCommentDomainEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("----- Handling UserLikedCommentDomainEvent: at {AppName} - ({@DomainEvent})", Program.AppName, notification);

            var post = await _postRepository.GetByIdAsync(notification.PostId);
            post.Like();

            var comment = await _commentRepository.GetByIdAsync(notification.CommentId);
            comment.Like();
        }
    }
}
