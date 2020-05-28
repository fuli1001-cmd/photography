using ApplicationMessages;
using ApplicationMessages.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.Post.API.Application.Commands.AppointmentDeal.AcceptAppointmentDeal;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.IntegrationEventHandlers
{
    public class OrderAcceptedEventHandler : IHandleMessages<OrderAcceptedEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OrderAcceptedEventHandler> _logger;

        public OrderAcceptedEventHandler(IMediator mediator, ILogger<OrderAcceptedEventHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(OrderAcceptedEvent message, IMessageHandlerContext context)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{message.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling OrderAcceptedEvent: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", message.Id, Program.AppName, message);

                var command = new AcceptAppointmentDealCommand { UserId = message.UserId, DealId = message.DealId };

                await _mediator.Send(command);
            }
        }
    }
}
