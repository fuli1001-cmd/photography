using ApplicationMessages.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.Notification.API.Application.Commands;
using Photography.Services.Notification.API.Application.Commands.CreateEvent;
using Photography.Services.Notification.Domain.AggregatesModel.EventAggregate;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Notification.API.Application.IntegrationEventHandlers
{
    public class PostRepliedEventHandler : IHandleMessages<PostRepliedEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PostRepliedEventHandler> _logger;

        public PostRepliedEventHandler(IMediator mediator, ILogger<PostRepliedEventHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(PostRepliedEvent message, IMessageHandlerContext context)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{message.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling PostRepliedEvent: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", message.Id, Program.AppName, message);
                
                var command = new CreateEventCommand
                {
                    FromUserId = message.FromUserId,
                    ToUserId = message.ToUserId,
                    PostId = message.PostId,
                    CommentText = message.Text,
                    EventType = EventType.ReplyPost
                };

                await _mediator.Send(command);
            }
        }
    }
}
