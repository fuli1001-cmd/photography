using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Messages.Events;
using Photography.Services.Order.API.Application.Commands.AcceptOrder;
using Photography.Services.Order.API.Application.Commands.CancelOrder;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Order.API.Application.IntegrationEvents
{
    public class AppointmentDealAcceptedEventHandler : IHandleMessages<AppointmentDealAcceptedEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AppointmentDealAcceptedEventHandler> _logger;

        public AppointmentDealAcceptedEventHandler(IMediator mediator, ILogger<AppointmentDealAcceptedEventHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(AppointmentDealAcceptedEvent message, IMessageHandlerContext context)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{message.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", message.Id, Program.AppName, message);

                var command = new AcceptOrderCommand { DealId = message.DealId };

                await _mediator.Send(command);
            }
        }
    }
}
