using ApplicationMessages.Events;
using Arise.DDD.API.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NServiceBus;
using Photography.Services.Notification.API.Application.Commands.CreateEvent;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Notification.API.Application.IntegrationEventHandlers
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

                // 创建约拍交易已收到事件，ToUserId是收到交易的用户id，FromUserId是发送交易的用户id
                var receivedCommand = new CreateEventCommand
                {
                    FromUserId = message.User1Id,
                    ToUserId = message.User2Id,
                    PostId = message.DealId,
                    EventType = Domain.AggregatesModel.EventAggregate.EventType.AppointmentDealReceived
                };

                await _mediator.Send(receivedCommand);

                // 创建约拍交易已发送事件，ToUserId是发送交易的用户id，FromUserId是收到交易的用户id
                var sentCommand = new CreateEventCommand
                {
                    FromUserId = message.User2Id,
                    ToUserId = message.User1Id,
                    PostId = message.DealId,
                    EventType = Domain.AggregatesModel.EventAggregate.EventType.AppointmentDealSent
                };

                await _mediator.Send(sentCommand);
            }
        }
    }
}
