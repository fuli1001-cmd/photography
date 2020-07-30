using ApplicationMessages.Events.Post;
using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.Notification.API.Application.Commands.CreateEvent;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Notification.API.Application.IntegrationEventHandlers
{
    public class CircleOwnerChangedEventHandler : IHandleMessages<CircleOwnerChangedEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CircleOwnerChangedEventHandler> _logger;

        public CircleOwnerChangedEventHandler(IMediator mediator, ILogger<CircleOwnerChangedEventHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(CircleOwnerChangedEvent message, IMessageHandlerContext context)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{message.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling CircleOwnerChangedEvent: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", message.Id, Program.AppName, message);

                var command = new CreateEventCommand
                {
                    FromUserId = message.OldOwnerId,
                    ToUserId = message.NewOwnerId,
                    CircleId = message.CircleId,
                    CircleName = message.CircleName,
                    EventType = Domain.AggregatesModel.EventAggregate.EventType.CircleOwnerChanged
                };

                await _mediator.Send(command);
            }
        }
    }
}
