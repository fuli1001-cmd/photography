using MediatR;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.Domain.AggregatesModel.CircleAggregate;
using Photography.Services.Post.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.DomainEventHandlers.QuittedCircle
{
    public class QuittedCircleDomainEventHandler : INotificationHandler<QuittedCircleDomainEvent>
    {
        private readonly ICircleRepository _circleRepository;
        private readonly ILogger<QuittedCircleDomainEventHandler> _logger;

        public QuittedCircleDomainEventHandler(
            ICircleRepository circleRepository,
            ILogger<QuittedCircleDomainEventHandler> logger)
        {
            _circleRepository = circleRepository ?? throw new ArgumentNullException(nameof(circleRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(QuittedCircleDomainEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("----- Handling QuittedCircleDomainEvent: at {AppName} - ({@DomainEvent})", Program.AppName, notification);

            var circle = await _circleRepository.GetByIdAsync(notification.CircleId);
            circle.DecreaseUserCount();
        }
    }
}
