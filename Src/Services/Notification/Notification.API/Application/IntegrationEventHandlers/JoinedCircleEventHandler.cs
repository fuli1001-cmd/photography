using ApplicationMessages.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.Notification.API.Application.Commands.CreateEvent;
using Photography.Services.Notification.API.Application.Commands.ProcessEvent;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Notification.API.Application.IntegrationEventHandlers
{
    public class JoinedCircleEventHandler : IHandleMessages<JoinedCircleEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<JoinedCircleEventHandler> _logger;

        public JoinedCircleEventHandler(IMediator mediator, ILogger<JoinedCircleEventHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(JoinedCircleEvent message, IMessageHandlerContext context)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{message.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling JoinedCircleEvent: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", message.Id, Program.AppName, message);

                // 创建用户已入圈的事件
                var createEventCommand = new CreateEventCommand
                {
                    FromUserId = message.JoinedUserId,
                    ToUserId = message.CircleOwnerId,
                    CircleName = message.CircleName,
                    EventType = Domain.AggregatesModel.EventAggregate.EventType.JoinCircle
                };

                await _mediator.Send(createEventCommand);

                // 将申请入圈事件标记为已处理
                var processEventCommand = new ProcessEventCommand
                {
                    FromUserId = message.JoinedUserId,
                    ToUserId = message.CircleOwnerId,
                    EventType = Domain.AggregatesModel.EventAggregate.EventType.ApplyJoinCircle
                };

                await _mediator.Send(processEventCommand);
            }
        }
    }
}
