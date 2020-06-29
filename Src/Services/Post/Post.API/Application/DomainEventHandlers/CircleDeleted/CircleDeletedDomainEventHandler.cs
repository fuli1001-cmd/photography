using MediatR;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.Domain.AggregatesModel.UserCircleRelationAggregate;
using Photography.Services.Post.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.DomainEventHandlers.CircleDeleted
{
    public class CircleDeletedDomainEventHandler : INotificationHandler<CircleDeletedDomainEvent>
    {
        private readonly IUserCircleRelationRepository _userCircleRelationRepository;
        private readonly ILogger<CircleDeletedDomainEventHandler> _logger;

        public CircleDeletedDomainEventHandler(
            IUserCircleRelationRepository userCircleRelationRepository,
            ILogger<CircleDeletedDomainEventHandler> logger)
        {
            _userCircleRelationRepository = userCircleRelationRepository ?? throw new ArgumentNullException(nameof(userCircleRelationRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(CircleDeletedDomainEvent notification, CancellationToken cancellationToken)
        {
            var relations = await _userCircleRelationRepository.GetRelationsByCircleIdAsync(notification.CircleId);
            relations.ForEach(r => _userCircleRelationRepository.Remove(r));
        }
    }
}
