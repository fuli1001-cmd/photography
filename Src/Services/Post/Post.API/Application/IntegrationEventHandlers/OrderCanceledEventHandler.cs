using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Messages.Events;
using Photography.Services.Post.API.Application.Commands.AppointmentDeal.CancelAppointmentDeal;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.IntegrationEventHandlers
{
    public class OrderCanceledEventHandler : IHandleMessages<OrderCanceledEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OrderCanceledEventHandler> _logger;

        public OrderCanceledEventHandler(IMediator mediator, ILogger<OrderCanceledEventHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(OrderCanceledEvent message, IMessageHandlerContext context)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{message.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling OrderCanceledEvent: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", message.Id, Program.AppName, message);

                var command = new CancelAppointmentDealCommand { UserId = message.UserId, DealId = message.DealId };

                await _mediator.Send(command);
            }
        }
    }
}
