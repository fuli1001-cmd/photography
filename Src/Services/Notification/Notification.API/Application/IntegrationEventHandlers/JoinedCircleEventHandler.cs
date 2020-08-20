using ApplicationMessages.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.Notification.API.Application.Commands.CreateEvent;
using Photography.Services.Notification.API.Application.Commands.ProcessEvent;
using Photography.Services.Notification.Domain.AggregatesModel.UserAggregate;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Notification.API.Application.IntegrationEventHandlers
{
    public class JoinedCircleEventHandler : IHandleMessages<JoinedCircleEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;
        private readonly ILogger<JoinedCircleEventHandler> _logger;

        public JoinedCircleEventHandler(IUserRepository userRepository, IMediator mediator, ILogger<JoinedCircleEventHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(JoinedCircleEvent message, IMessageHandlerContext context)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{message.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling JoinedCircleEvent: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", message.Id, Program.AppName, message);

                var fromUser = await _userRepository.GetByIdAsync(message.JoinedUserId);

                #region 给圈主发通知
                var createEventCommand = new CreateEventCommand
                {
                    FromUserId = message.JoinedUserId,
                    ToUserId = message.CircleOwnerId,
                    CircleId = message.CircleId,
                    CircleName = message.CircleName,
                    EventType = Domain.AggregatesModel.EventAggregate.EventType.JoinCircle,
                    PushMessage = $"{fromUser.Nickname}加入{message.CircleName}"
                };

                await _mediator.Send(createEventCommand);
                #endregion

                #region 给申请入圈用户发通知
                var joinCircleAcceptedCommand = new CreateEventCommand
                {
                    FromUserId = message.CircleOwnerId,
                    ToUserId = message.JoinedUserId,
                    CircleId = message.CircleId,
                    CircleName = message.CircleName,
                    EventType = Domain.AggregatesModel.EventAggregate.EventType.ApplyJoinCircleAccepted,
                    PushMessage = $"申请通过，已加入{message.CircleName}"
                };

                await _mediator.Send(joinCircleAcceptedCommand);
                #endregion

                #region 将申请入圈事件标记为已处理
                var processEventCommand = new ProcessEventCommand
                {
                    FromUserId = message.JoinedUserId,
                    ToUserId = message.CircleOwnerId,
                    EventType = Domain.AggregatesModel.EventAggregate.EventType.ApplyJoinCircle
                };

                await _mediator.Send(processEventCommand);
                #endregion
            }
        }
    }
}
