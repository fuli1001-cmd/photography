using ApplicationMessages.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.User.Domain.AggregatesModel.UserRelationAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.User.ToggleUserFollow
{
    public class ToggleUserFollowCommandHandler : IRequestHandler<ToggleUserFollowCommand, bool>
    {
        private readonly IUserRelationRepository _userRelationRepository;
        private readonly ILogger<ToggleUserFollowCommandHandler> _logger;
        private readonly IServiceProvider _serviceProvider;

        private IMessageSession _messageSession;

        public ToggleUserFollowCommandHandler(IUserRelationRepository userRelationRepository,
            IServiceProvider serviceProvider,
            ILogger<ToggleUserFollowCommandHandler> logger)
        {
            _userRelationRepository = userRelationRepository ?? throw new ArgumentNullException(nameof(userRelationRepository));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ToggleUserFollowCommand request, CancellationToken cancellationToken)
        {
            bool result = false;

            var ur = await _userRelationRepository.GetAsync(request.FollowerId, request.UserIdToFollow);

            if (ur == null)
            {
                ur = new UserRelation(request.FollowerId, request.UserIdToFollow);
                ur.Follow();
                _userRelationRepository.Add(ur);
                result = await _userRelationRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

                if (result)
                    await SendUserFollowedEventAsync(request.FollowerId, request.UserIdToFollow);
            }
            else
            {
                if (ur.Followed)
                    ur.UnFollow();
                else
                    ur.Follow();

                result = await _userRelationRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

                if (result)
                    await SendUserUnFollowedEventAsync(request.FollowerId, request.UserIdToFollow);
            }

            // 返回被关注者是否关注了我
            if (result)
            {
                ur = await _userRelationRepository.GetAsync(request.UserIdToFollow, request.FollowerId);
                return ur?.Followed ?? false;
            }
            else
                throw new ApplicationException("操作失败");
        }

        private async Task SendUserFollowedEventAsync(Guid followerId, Guid followedUserId)
        {
            var @event = new UserFollowedEvent { FollowerId = followerId, FollowedUserId = followedUserId };
            await SendEvent(@event);
        }

        private async Task SendUserUnFollowedEventAsync(Guid followerId, Guid followedUserId)
        {
            var @event = new UserUnFollowedEvent { FollowerId = followerId, FollowedUserId = followedUserId };
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
