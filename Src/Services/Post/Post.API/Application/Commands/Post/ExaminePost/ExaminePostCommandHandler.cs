using ApplicationMessages.Events.Post;
using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.Post.API.Query.Interfaces;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using Photography.Services.Post.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.Post.ExaminePost
{
    public class ExaminePostCommandHandler : IRequestHandler<ExaminePostCommand, bool>
    {
        private readonly IPostQueries _postQueries;
        private readonly IPostRepository _postRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<ExaminePostCommandHandler> _logger;
        private readonly IServiceProvider _serviceProvider;

        private IMessageSession _messageSession;

        public ExaminePostCommandHandler(
            IPostQueries postQueries, 
            IPostRepository postRepository,
            IUserRepository userRepository,
            IServiceProvider serviceProvider,
            ILogger<ExaminePostCommandHandler> logger)
        {
            _postQueries = postQueries ?? throw new ArgumentNullException(nameof(postQueries));
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ExaminePostCommand request, CancellationToken cancellationToken)
        {
            var post = await _postRepository.GetByIdAsync(request.PostId);
            post.Examine(request.PostAuthStatus);

            if (await _postRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
            {
                // 如果审核通过，给帖子中被@的用户发送被@通知
                if (request.PostAuthStatus == PostAuthStatus.Authenticated)
                {
                    var atUserIds = await _postQueries.GetAtUserIdsAsync(post);
                    await SendUserAtedEventAsync(post.UserId, post.Id, atUserIds);
                }

                return true;
            }

            throw new ApplicationException("操作失败");
        }

        // 发送用户被@事件
        private async Task SendUserAtedEventAsync(Guid postUserId, Guid postId, IEnumerable<Guid> atUserIds)
        {
            if (atUserIds.Count() > 0)
            {
                var @event = new UserAtedEvent { PostUserId = postUserId, PostId = postId, AtUserIds = atUserIds };

                _messageSession = (IMessageSession)_serviceProvider.GetService(typeof(IMessageSession));
                await _messageSession.Publish(@event);

                _logger.LogInformation("----- Published PostPublishedEvent: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);
            }
        }
    }
}
