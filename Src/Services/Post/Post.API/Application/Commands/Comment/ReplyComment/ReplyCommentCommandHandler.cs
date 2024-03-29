﻿using ApplicationMessages.Events;
using Arise.DDD.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.Post.Domain.AggregatesModel.CommentAggregate;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using Photography.Services.Post.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.Comment.ReplyComment
{
    public class ReplyCommentCommandHandler : IRequestHandler<ReplyCommentCommand, int>
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IPostRepository _postRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ReplyCommentCommandHandler> _logger;
        private readonly IServiceProvider _serviceProvider;

        private IMessageSession _messageSession;

        public ReplyCommentCommandHandler(
            ICommentRepository commentRepository,
            IPostRepository postRepository,
            IUserRepository userRepository,
            IServiceProvider serviceProvider,
            IHttpContextAccessor httpContextAccessor, 
            ILogger<ReplyCommentCommandHandler> logger)
        {
            _commentRepository = commentRepository ?? throw new ArgumentNullException(nameof(commentRepository));
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<int> Handle(ReplyCommentCommand request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var parentComment = await _commentRepository.GetByIdAsync(request.CommentId);
            var comment = new Domain.AggregatesModel.CommentAggregate.Comment();
            comment.ReplyComment(request.Text, parentComment.PostId, request.CommentId, userId);
            _commentRepository.Add(comment);

            if (await _commentRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
            {
                await SendCommentRepliedEventAsync(userId, parentComment.UserId, comment.Id, parentComment.PostId, request.Text);

                //返回帖子评论的总数量
                var post = await _postRepository.GetByIdAsync(parentComment.PostId);
                return post.CommentCount;
            }

            throw new ApplicationException("操作失败");
        }

        private async Task SendCommentRepliedEventAsync(Guid fromUserId, Guid toUserId, Guid commentId, Guid postId, string text)
        {
            var @event = new CommentRepliedEvent
            {
                FromUserId = fromUserId,
                ToUserId = toUserId,
                CommentId = commentId,
                PostId = postId,
                Text = text
            };

            _messageSession = (IMessageSession)_serviceProvider.GetService(typeof(IMessageSession));
            await _messageSession.Publish(@event);
            _logger.LogInformation("----- Published CommentRepliedEvent: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);
        }
    }
}
