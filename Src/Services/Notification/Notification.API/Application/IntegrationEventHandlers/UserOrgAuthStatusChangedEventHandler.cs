using ApplicationMessages.Events.User;
using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.Notification.API.Application.Commands.CreateEvent;
using Photography.Services.Notification.API.Application.Commands.User.SetOrgAuthStatus;
using Photography.Services.Notification.Domain.AggregatesModel.EventAggregate;
using Photography.Services.Notification.Domain.AggregatesModel.UserAggregate;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Notification.API.Application.IntegrationEventHandlers
{
    public class UserOrgAuthStatusChangedEventHandler : IHandleMessages<UserOrgAuthStatusChangedEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UserOrgAuthStatusChangedEventHandler> _logger;

        public UserOrgAuthStatusChangedEventHandler(IMediator mediator, ILogger<UserOrgAuthStatusChangedEventHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(UserOrgAuthStatusChangedEvent message, IMessageHandlerContext context)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{message.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling UserOrgAuthStatusChangedEvent: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", message.Id, Program.AppName, message);

                #region 设置用户团体认证状态
                var command = new SetOrgAuthStatusCommand { UserId = message.UserId, Status = (AuthStatus)message.Status };
                await _mediator.Send(command);
                #endregion

                #region 通知用户
                if (message.Status == 2 || message.Status == 3)
                {
                    var eventCommand = new CreateEventCommand
                    {
                        ToUserId = message.UserId,
                        EventType = message.Status == 2 ? EventType.UserOrgAuthSuccess : EventType.UserOrgAuthFailed,
                        PushMessage = message.Status == 2 ? "团体认证成功" : "团体认证未通过"
                    };

                    await _mediator.Send(eventCommand);
                }
                #endregion
            }
        }
    }
}
