using ApplicationMessages.Events;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.User.Domain.AggregatesModel.UserAggregate;
using Photography.Services.User.Domain.AggregatesModel.UserRelationAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.ToggleFollow
{
    public class ToggleFollowCommandHandler : IRequestHandler<ToggleFollowCommand, bool>
    {
        private readonly IUserRelationRepository _userRelationRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ToggleFollowCommandHandler> _logger;
        private readonly IServiceProvider _serviceProvider;

        private IMessageSession _messageSession;

        public ToggleFollowCommandHandler(IUserRelationRepository userRelationRepository,
            IServiceProvider serviceProvider,
            IHttpContextAccessor httpContextAccessor, 
            ILogger<ToggleFollowCommandHandler> logger)
        {
            _userRelationRepository = userRelationRepository ?? throw new ArgumentNullException(nameof(userRelationRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ToggleFollowCommand request, CancellationToken cancellationToken)
        {
            bool result = false;

            var userId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var ur = await _userRelationRepository.GetAsync(userId, request.UserIdToFollow);

            if (ur == null)
            {
                ur = new UserRelation(userId, request.UserIdToFollow);
                _userRelationRepository.Add(ur);
                result = await _userRelationRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
                
                if (result)
                    await SendUserFollowedEventAsync(userId, request.UserIdToFollow);
            }
            else
            {
                _userRelationRepository.Remove(ur);
                result = await _userRelationRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

                if (result)
                    await SendUserUnFollowedEventAsync(userId, request.UserIdToFollow);
            }

            return result;
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
