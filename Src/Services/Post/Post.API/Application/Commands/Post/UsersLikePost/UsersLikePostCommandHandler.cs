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

namespace Photography.Services.Post.API.Application.Commands.Post.UsersLikePost
{
    public class UsersLikePostCommandHandler : IRequestHandler<UserLikePostCommand, bool>
    {
        private readonly IUserPostRelationRepository _userPostRelationRepository;
        private readonly IPostRepository _postRepository;
        private readonly ILogger<UsersLikePostCommandHandler> _logger;
        private readonly IServiceProvider _serviceProvider;

        private IMessageSession _messageSession;

        public UsersLikePostCommandHandler(
            IUserPostRelationRepository userPostRelationRepository,
            IPostRepository postRepository,
            IServiceProvider serviceProvider,
            ILogger<UsersLikePostCommandHandler> logger)
        {
            _userPostRelationRepository = userPostRelationRepository ?? throw new ArgumentNullException(nameof(userPostRelationRepository));
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(UserLikePostCommand request, CancellationToken cancellationToken)
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

            return result;
        }

        private async Task SendPostLikedEventAsync(Guid postId, Guid likingUserId)
        {
            var post = await _postRepository.GetByIdAsync(postId);
            var @event = new PostLikedEvent { PostUserId = post.UserId, LikingUserId = likingUserId, PostId = postId };

            _messageSession = (IMessageSession)_serviceProvider.GetService(typeof(IMessageSession));
            await _messageSession.Publish(@event);

            _logger.LogInformation("----- Published PostLikedEvent: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);
        }
    }
}
