using MediatR;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.Domain.AggregatesModel.CircleAggregate;
using Photography.Services.Post.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.DomainEventHandlers.JoinedCircle
{
    public class JoinedCircleDomainEventHandler : INotificationHandler<JoinedCircleDomainEvent>
    {
        private readonly ICircleRepository _circleRepository;
        private readonly ILogger<JoinedCircleDomainEventHandler> _logger;

        public JoinedCircleDomainEventHandler(
            ICircleRepository circleRepository,
            ILogger<JoinedCircleDomainEventHandler> logger)
        {
            _circleRepository = circleRepository ?? throw new ArgumentNullException(nameof(circleRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(JoinedCircleDomainEvent notification, CancellationToken cancellationToken)
        {
            var circle = await _circleRepository.GetByIdAsync(notification.CircleId);
            circle.IncreaseUserCount();
        }
    }
}
