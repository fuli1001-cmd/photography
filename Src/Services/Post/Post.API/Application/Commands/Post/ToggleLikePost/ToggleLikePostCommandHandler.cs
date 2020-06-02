using ApplicationMessages.Events;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using Photography.Services.Post.Domain.AggregatesModel.UserPostRelationAggregate;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.Post.ToggleLikePost
{
    public class ToggleLikePostCommandHandler : IRequestHandler<ToggleLikePostCommand, bool>
    {
        private readonly IUserPostRelationRepository _userPostRelationRepository;
        private readonly IPostRepository _postRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ToggleLikePostCommandHandler> _logger;
        private readonly IServiceProvider _serviceProvider;

        private IMessageSession _messageSession;

        public ToggleLikePostCommandHandler(IUserPostRelationRepository userPostRelationRepository,
            IPostRepository postRepository,
            IServiceProvider serviceProvider, 
            IHttpContextAccessor httpContextAccessor, 
            ILogger<ToggleLikePostCommandHandler> logger)
        {
            _userPostRelationRepository = userPostRelationRepository ?? throw new ArgumentNullException(nameof(userPostRelationRepository));
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ToggleLikePostCommand request, CancellationToken cancellationToken)
        {
            bool result = false;

            var userId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userPostRelation = await _userPostRelationRepository.GetAsync(userId, request.PostId, UserPostRelationType.Like);

            if (userPostRelation == null)
            {
                userPostRelation = new UserPostRelation(userId, request.PostId);
                userPostRelation.Like();
                _userPostRelationRepository.Add(userPostRelation);

                result = await _userPostRelationRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
                if (result)
                    await SendPostLikedEventAsync(request.PostId, userId);
            }
            else
            {
                userPostRelation.UnLike();
                _userPostRelationRepository.Remove(userPostRelation);

                result = await _userPostRelationRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
                if (result)
                    await SendPostUnLikedEventAsync(request.PostId, userId);
            }

            return result;
        }

        private async Task SendPostLikedEventAsync(Guid postId, Guid likingUserId)
        {
            var post = await _postRepository.GetByIdAsync(postId);
            var @event = new PostLikedEvent { PostUserId = post.UserId, LikingUserId = likingUserId };
            await SendEvent(@event);
        }

        private async Task SendPostUnLikedEventAsync(Guid postId, Guid likingUserId)
        {
            var post = await _postRepository.GetByIdAsync(postId);
            var @event = new PostUnLikedEvent { PostUserId = post.UserId, LikingUserId = likingUserId };
            await SendEvent(@event);
        }

        private async Task SendEvent(BaseEvent @event)
        {
            _messageSession = (IMessageSession)_serviceProvider.GetService(typeof(IMessageSession));
            await _messageSession.Publish(@event);
            _logger.LogInformation("----- Published {IntegrationEventName}: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", @event.GetType().Name, @event.Id, Program.AppName, @event);
        }
    }
}
