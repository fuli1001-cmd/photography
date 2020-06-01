using MediatR;
using Microsoft.Extensions.Logging;
using Photography.Services.User.Domain.AggregatesModel.UserAggregate;
using Photography.Services.User.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.DomainEventHandlers.FollowedUser
{
    public class FollowedUserDomainEventHandler : INotificationHandler<FollowedUserDomainEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<FollowedUserDomainEventHandler> _logger;

        public FollowedUserDomainEventHandler(
            IUserRepository userRepository,
            ILogger<FollowedUserDomainEventHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(FollowedUserDomainEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("----- Handling FollowedUserDomainEvent: at {AppName} - ({@DomainEvent})", Program.AppName, notification);

            // 关注者增加关注数量
            var follower = await _userRepository.GetByIdAsync(notification.FollowerId);
            follower.IncreaseFollowingCount();

            // 被关注者增加粉丝数量
            var followedUser = await _userRepository.GetByIdAsync(notification.FollowedUserId);
            followedUser.IncreaseFollowerCount();
        }
    }
}
