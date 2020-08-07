using ApplicationMessages.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.Post.Domain.AggregatesModel.CommentAggregate;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.Comment.UserReplyPost
{
    public class UserReplyPostCommandHandler : IRequestHandler<UserReplyPostCommand, int>
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IPostRepository _postRepository;
        private readonly ILogger<UserReplyPostCommandHandler> _logger;
        private readonly IServiceProvider _serviceProvider;

        private IMessageSession _messageSession;

        public UserReplyPostCommandHandler(ICommentRepository commentRepository,
            IPostRepository postRepository,
            IServiceProvider serviceProvider,
            ILogger<UserReplyPostCommandHandler> logger)
        {
            _commentRepository = commentRepository ?? throw new ArgumentNullException(nameof(commentRepository));
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<int> Handle(UserReplyPostCommand request, CancellationToken cancellationToken)
        {
            var comment = new Domain.AggregatesModel.CommentAggregate.Comment();
            comment.ReplyPost(request.Text, request.PostId, request.RepliedUserId);
            _commentRepository.Add(comment);

            if (await _commentRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
            {
                var post = await _postRepository.GetByIdAsync(request.PostId);

                await SendPostRepliedEventAsync(post, request.RepliedUserId, request.Text);

                //返回帖子评论的总数量
                return post.CommentCount;
            }

            throw new ApplicationException("操作失败");
        }

        private async Task SendPostRepliedEventAsync(Domain.AggregatesModel.PostAggregate.Post post, Guid repliedUserId, string text)
        {
            var @event = new PostRepliedEvent
            {
                FromUserId = repliedUserId,
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
