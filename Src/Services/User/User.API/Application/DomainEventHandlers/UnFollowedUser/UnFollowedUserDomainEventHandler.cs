using MediatR;
using Microsoft.Extensions.Logging;
using Photography.Services.User.Domain.AggregatesModel.UserAggregate;
using Photography.Services.User.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.DomainEventHandlers.UnFollowedUser
{
    public class UnFollowedUserDomainEventHandler : INotificationHandler<UnFollowedUserDomainEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UnFollowedUserDomainEventHandler> _logger;

        public UnFollowedUserDomainEventHandler(
            IUserRepository userRepository,
            ILogger<UnFollowedUserDomainEventHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(UnFollowedUserDomainEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("----- Handling UnFollowedUserDomainEvent: at {AppName} - ({@DomainEvent})", Program.AppName, notification);

            // 关注者减少关注数量
            var follower = await _userRepository.GetByIdAsync(notification.FollowerId);
            follower.DecreaseFollowingCount();

            // 被关注者减少粉丝数量
            var followedUser = await _userRepository.GetByIdAsync(notification.FollowedUserId);
            followedUser.DecreaseFollowerCount();
        }
    }
}
