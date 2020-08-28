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

namespace Photography.Services.Notification.API.Application.IntegrationEventHandlers.Post
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

                var msg = "您收到一条约拍请求，请及时处理";

                // 创建收到约拍请求的事件
                var command = new CreateEventCommand
                {
                    FromUserId = message.User1Id,
                    ToUserId = message.User2Id,
                    EventType = Domain.AggregatesModel.EventAggregate.EventType.AppointmentDealCreated,
                    CommentText = msg,
                    PushMessage = msg
                };

                await _mediator.Send(command);
            }
        }
    }
}
