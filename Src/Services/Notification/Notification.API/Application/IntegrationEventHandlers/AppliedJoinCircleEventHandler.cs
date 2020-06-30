using ApplicationMessages.Events;
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
    public class AppliedJoinCircleEventHandler : IHandleMessages<AppliedJoinCircleEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AppliedJoinCircleEventHandler> _logger;

        public AppliedJoinCircleEventHandler(IMediator mediator, ILogger<AppliedJoinCircleEventHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(AppliedJoinCircleEvent message, IMessageHandlerContext context)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{message.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling AppliedJoinCircleEvent: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", message.Id, Program.AppName, message);

                var command = new CreateEventCommand
                {
                    FromUserId = message.ApplyUserId,
                    ToUserId = message.CircleOwnerId,
                    CircleId = message.CircleId,
                    CircleName = message.CircleName,
                    EventType = Domain.AggregatesModel.EventAggregate.EventType.ApplyJoinCircle
                };

                await _mediator.Send(command);
            }
        }
    }
}
