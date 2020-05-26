using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Messages.Events;
using Photography.Services.Order.API.Application.Commands.CancelOrder;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Order.API.Application.IntegrationEvents
{
    public class AppointmentDealCanceledEventHandler : IHandleMessages<AppointmentDealCanceledEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AppointmentDealCanceledEventHandler> _logger;

        public AppointmentDealCanceledEventHandler(IMediator mediator, ILogger<AppointmentDealCanceledEventHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(AppointmentDealCanceledEvent message, IMessageHandlerContext context)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{message.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", message.Id, Program.AppName, message);

                var command = new CancelOrderCommand { DealId = message.DealId };

                await _mediator.Send(command);
            }
        }
    }
}
