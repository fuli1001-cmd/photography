using ApplicationMessages.Events;
using Arise.DDD.API.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NServiceBus;
using Photography.Services.Order.API.Application.Commands.CreateOrder;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Order.API.Application.IntegrationEventHandlers
{
    public class AppointmentDealCreatedEventHandler : IHandleMessages<AppointmentDealCreatedEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AppointmentDealCreatedEventHandler> _logger;

        public AppointmentDealCreatedEventHandler(IMediator mediator, ILogger<AppointmentDealCreatedEventHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(AppointmentDealCreatedEvent message, IMessageHandlerContext context)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{message.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling AppointmentDealCreatedEvent: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", message.Id, Program.AppName, message);

                var command = new CreateOrderCommand
                {
                    User1Id = message.User1Id,
                    User2Id = message.User2Id,
                    DealId = message.DealId,
                    AppointmentedUserType = message.AppointmentedUserType,
                    PayerType = message.PayerType,
                    Price = message.Price,
                    AppointedTime = message.AppointedTime,
                    PayerId = message.PayerId,
                    Text = message.Text,
                    Latitude = message.Latitude,
                    Longitude = message.Longitude,
                    LocationName = message.LocationName,
                    Address = message.Address
                };

                await _mediator.Send(command);
            }
        }
    }
}
