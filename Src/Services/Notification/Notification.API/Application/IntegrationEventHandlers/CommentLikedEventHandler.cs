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
    public class CommentLikedEventHandler : IHandleMessages<CommentLikedEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CommentLikedEventHandler> _logger;

        public CommentLikedEventHandler(IMediator mediator, ILogger<CommentLikedEventHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(CommentLikedEvent message, IMessageHandlerContext context)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{message.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling CommentLikedEvent: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", message.Id, Program.AppName, message);

                var command = new CreateEventCommand
                {
                    FromUserId = message.LikingUserId,
                    ToUserId = message.CommentUserId,
                    PostId = message.PostId,
                    CommentId = message.CommentId,
                    CommentText = message.CommentText,
                    EventType = EventType.LikeComment
                };

                await _mediator.Send(command);
            }
        }
    }
}
