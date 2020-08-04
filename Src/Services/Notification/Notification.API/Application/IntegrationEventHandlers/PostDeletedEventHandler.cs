using ApplicationMessages.Events;
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
    public class PostDeletedEventHandler : IHandleMessages<PostDeletedEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PostDeletedEventHandler> _logger;

        public PostDeletedEventHandler(IMediator mediator, ILogger<PostDeletedEventHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(PostDeletedEvent message, IMessageHandlerContext context)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{message.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling PostDeletedEvent: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", message.Id, Program.AppName, message);

                var command = new CreateEventCommand
                {
                    FromUserId = message.OperatorId,
                    ToUserId = message.UserId,
                    EventType = EventType.DeletePost,
                    PushMessage = "你发布的内容因违反平台规则，已被删除，请发布合规内容"
                };

                await _mediator.Send(command);
            }
        }
    }
}
