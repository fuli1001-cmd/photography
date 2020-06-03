using ApplicationMessages.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.Notification.API.Application.Commands.CreateEvent;
using Photography.Services.Notification.API.Application.Commands.User.Follow;
using Photography.Services.Notification.Domain.AggregatesModel.EventAggregate;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Notification.API.Application.IntegrationEventHandlers
{
    public class UserFollowedEventHandler : IHandleMessages<UserFollowedEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UserFollowedEventHandler> _logger;

        public UserFollowedEventHandler(IMediator mediator, ILogger<UserFollowedEventHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(UserFollowedEvent message, IMessageHandlerContext context)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{message.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling UserFollowedEvent: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", message.Id, Program.AppName, message);

                var followCommand = new FollowCommand { FollowerId = message.FollowerId, FollowedUserId = message.FollowedUserId };
                await _mediator.Send(followCommand);

                var eventCommand = new CreateEventCommand
                {
                    FromUserId = message.FollowerId,
                    ToUserId = message.FollowedUserId,
                    EventType = EventType.Follow
                };
                await _mediator.Send(eventCommand);
            }
        }
    }
}
