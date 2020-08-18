using ApplicationMessages.Events.User;
using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.Notification.API.Application.Commands.CreateEvent;
using Photography.Services.Notification.Domain.AggregatesModel.EventAggregate;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Notification.API.Application.IntegrationEventHandlers
{
    public class UserFeedbackCreatedEventHandler : IHandleMessages<UserFeedbackCreatedEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UserFeedbackCreatedEventHandler> _logger;

        public UserFeedbackCreatedEventHandler(IMediator mediator, ILogger<UserFeedbackCreatedEventHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(UserFeedbackCreatedEvent message, IMessageHandlerContext context)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{message.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling UserFeedbackCreatedEvent: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", message.Id, Program.AppName, message);

                var command = new CreateEventCommand
                {
                    FromUserId = message.UserId, // 注：本事件无FromUserId，此处设置FromUserId为接收通知用户的id是因为FromUserId不能为空
                    ToUserId = message.UserId,
                    EventType = EventType.FeedbackCreated,
                    PushMessage = "已收到您的意见反馈，我们将尽快处理，感谢您对我们的支持。"
                };

                await _mediator.Send(command);
            }
        }
    }
}
