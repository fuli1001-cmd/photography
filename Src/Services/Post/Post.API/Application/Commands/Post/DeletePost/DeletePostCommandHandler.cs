using ApplicationMessages.Events;
using Arise.DDD.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.Post.DeletePost
{
    public class DeletePostCommandHandler : IRequestHandler<DeletePostCommand, bool>
    {
        private readonly IPostRepository _postRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<DeletePostCommandHandler> _logger;
        private readonly IServiceProvider _serviceProvider;

        private IMessageSession _messageSession;

        public DeletePostCommandHandler(IPostRepository postRepository, 
            IHttpContextAccessor httpContextAccessor,
            IServiceProvider serviceProvider,
            ILogger<DeletePostCommandHandler> logger)
        {
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(DeletePostCommand request, CancellationToken cancellationToken)
        {
            Guid userId = Guid.Empty;

            var post = await _postRepository.GetPostWithNavigationPropertiesById(request.PostId);
            if (post == null)
                throw new ClientException("操作失败", new List<string> { $"Post {request.PostId} does not exists." });

            // 历史原因：
            // 没有管理平台之前只有手机用户操作自己的数据，因此DeletePostCommand中没有要求传入UserId
            // 管理平台加入之后也是用这个API，但是管理平台必须传UserId，才能知道是操作的哪个用户的数据
            var role = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
            if (role == "admin")
            {
                if (request.UserId == null)
                    throw new ClientException("操作失败", new List<string> { $"UserId is need." });

                userId = request.UserId.Value;
            }
            else
            {
                userId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            }

            // 当前用户不是发布该帖的用户，不能删除
            if (post.UserId != userId)
                throw new ClientException("操作失败", new List<string> { $"Post {post.Id} does not belong to user {userId}"});

            post.Delete();
            _postRepository.Update(post);
            _postRepository.Remove(post);

            if (await _postRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
            {
                await SendPostDeletedEventAsync(userId);
                return true;
            }
            
            throw new ApplicationException("操作失败");
        }

        private async Task SendPostDeletedEventAsync(Guid userId)
        {
            var operatorId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var @event = new PostDeletedEvent { UserId = userId, OperatorId = operatorId };
            _messageSession = (IMessageSession)_serviceProvider.GetService(typeof(IMessageSession));
            await _messageSession.Publish(@event);
            _logger.LogInformation("----- Published PostDeletedEvent: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);
        }
    }
}
