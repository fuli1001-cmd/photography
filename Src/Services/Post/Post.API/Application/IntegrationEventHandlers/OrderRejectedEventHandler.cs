using ApplicationMessages.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.Post.API.Application.Commands.AppointmentDeal.RejectAppointmentDeal;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.IntegrationEventHandlers
{
    public class OrderRejectedEventHandler : IHandleMessages<OrderRejectedEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OrderRejectedEventHandler> _logger;

        public OrderRejectedEventHandler(IMediator mediator, ILogger<OrderRejectedEventHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(OrderRejectedEvent message, IMessageHandlerContext context)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{message.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling OrderRejectedEvent: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", message.Id, Program.AppName, message);

                var command = new RejectAppointmentDealCommand { UserId = message.ProcessingUserId, DealId = message.DealId };

                await _mediator.Send(command);
            }
        }
    }
}
