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

namespace Photography.Services.Notification.API.Application.IntegrationEventHandlers.User
{
    public class IdAuthenticatedEventHandler : IHandleMessages<IdAuthenticatedEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<IdAuthenticatedEventHandler> _logger;

        public IdAuthenticatedEventHandler(IMediator mediator, ILogger<IdAuthenticatedEventHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(IdAuthenticatedEvent message, IMessageHandlerContext context)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{message.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling IdAuthenticatedEvent: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", message.Id, Program.AppName, message);

                // 创建实名认证结果的事件
                var createEventCommand = new CreateEventCommand
                {
                    FromUserId = message.OperatorId,
                    ToUserId = message.UserId,
                    EventType = message.Passed ? Domain.AggregatesModel.EventAggregate.EventType.IdAuthenticated : Domain.AggregatesModel.EventAggregate.EventType.IdRejected,
                    PushMessage = message.Passed ? "你已实名认证成功" : "你的实名认证未通过"
                };

                await _mediator.Send(createEventCommand);
            }
        }
    }
}
