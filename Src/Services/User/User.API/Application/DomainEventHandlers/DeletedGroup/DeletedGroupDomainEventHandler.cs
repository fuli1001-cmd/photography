using MediatR;
using Microsoft.Extensions.Logging;
using Photography.Services.User.Domain.AggregatesModel.GroupUserAggregate;
using Photography.Services.User.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.DomainEventHandlers.DeletedGroup
{
    public class DeletedGroupDomainEventHandler : INotificationHandler<DeletedGroupDomainEvent>
    {
        private readonly IGroupUserRepository _groupUserRepository;
        private readonly ILogger<DeletedGroupDomainEventHandler> _logger;

        public DeletedGroupDomainEventHandler(
            IGroupUserRepository groupUserRepository,
            ILogger<DeletedGroupDomainEventHandler> logger)
        {
            _groupUserRepository = groupUserRepository ?? throw new ArgumentNullException(nameof(groupUserRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(DeletedGroupDomainEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("----- Handling DeletedGroupDomainEvent: at {AppName} - ({@DomainEvent})", Program.AppName, notification);

            var groupUsers = await _groupUserRepository.GetGroupUsersByGroupIdAsync(notification.GroupId);
            groupUsers.ForEach(gu => _groupUserRepository.Remove(gu));
        }
    }
}
