using ApplicationMessages.Events;
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

namespace Photography.Services.Post.API.Application.Commands.Comment.ReplyPost
{
    public class ReplyPostCommandHandler : IRequestHandler<ReplyPostCommand, int>
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IPostRepository _postRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ReplyPostCommandHandler> _logger;
        private readonly IServiceProvider _serviceProvider;

        private IMessageSession _messageSession;

        public ReplyPostCommandHandler(ICommentRepository commentRepository,
            IPostRepository postRepository,
            IUserRepository userRepository,
            IServiceProvider serviceProvider,
            IHttpContextAccessor httpContextAccessor, 
            ILogger<ReplyPostCommandHandler> logger)
        {
            _commentRepository = commentRepository ?? throw new ArgumentNullException(nameof(commentRepository));
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<int> Handle(ReplyPostCommand request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var comment = new Domain.AggregatesModel.CommentAggregate.Comment();
            comment.ReplyPost(request.Text, request.PostId, userId);
            _commentRepository.Add(comment);
            
            if (await _commentRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
            {
                var post = await _postRepository.GetByIdAsync(request.PostId);
                
                await SendPostRepliedEventAsync(post, userId, request.Text);

                //返回帖子评论的总数量
                return post.CommentCount;
            }

            throw new DomainException("评论失败。");
        }

        private async Task SendPostRepliedEventAsync(Domain.AggregatesModel.PostAggregate.Post post, Guid userId, string text)
        {
            var @event = new PostRepliedEvent
            {
                FromUserId = userId,
                ToUserId = post.UserId,
                PostId = post.Id,
                Text = text
            };

            _messageSession = (IMessageSession)_serviceProvider.GetService(typeof(IMessageSession));
            await _messageSession.Publish(@event);
            _logger.LogInformation("----- Published PostRepliedEvent: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);
        }
    }
}
