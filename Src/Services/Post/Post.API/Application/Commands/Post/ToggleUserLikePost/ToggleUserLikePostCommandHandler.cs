using ApplicationMessages.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using Photography.Services.Post.Domain.AggregatesModel.UserPostRelationAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.Post.ToggleUserLikePost
{
    public class ToggleUserLikePostCommandHandler : IRequestHandler<ToggleUserLikePostCommand, bool>
    {
        private readonly IUserPostRelationRepository _userPostRelationRepository;
        private readonly IPostRepository _postRepository;
        private readonly ILogger<ToggleUserLikePostCommandHandler> _logger;
        private readonly IServiceProvider _serviceProvider;

        private IMessageSession _messageSession;

        public ToggleUserLikePostCommandHandler(
            IUserPostRelationRepository userPostRelationRepository,
            IPostRepository postRepository,
            IServiceProvider serviceProvider,
            ILogger<ToggleUserLikePostCommandHandler> logger)
        {
            _userPostRelationRepository = userPostRelationRepository ?? throw new ArgumentNullException(nameof(userPostRelationRepository));
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ToggleUserLikePostCommand request, CancellationToken cancellationToken)
        {
            bool result = false;

            var userPostRelation = await _userPostRelationRepository.GetAsync(request.UserId, request.PostId, UserPostRelationType.Like);

            if (userPostRelation == null)
            {
                userPostRelation = new UserPostRelation(request.UserId, request.PostId);
                userPostRelation.Like();
                _userPostRelationRepository.Add(userPostRelation);

                result = await _userPostRelationRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
                if (result)
                    await SendPostLikedEventAsync(request.PostId, request.UserId);
            }
            else
            {
                userPostRelation.UnLike();
                _userPostRelationRepository.Remove(userPostRelation);

                result = await _userPostRelationRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
                if (result)
                    await SendPostUnLikedEventAsync(request.PostId, request.UserId);
            }

            return result;
        }

        private async Task SendPostLikedEventAsync(Guid postId, Guid likingUserId)
        {
            var post = await _postRepository.GetByIdAsync(postId);

            var @event = new PostLikedEvent { PostUserId = post.UserId, LikingUserId = likingUserId, PostId = postId };
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
