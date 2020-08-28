using ApplicationMessages.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.Notification.API.Application.Commands.UnFollow;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Notification.API.Application.IntegrationEventHandlers.User
{
    public class UserUnFollowedEventHandler : IHandleMessages<UserUnFollowedEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UserUnFollowedEventHandler> _logger;

        public UserUnFollowedEventHandler(IMediator mediator, ILogger<UserUnFollowedEventHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(UserUnFollowedEvent message, IMessageHandlerContext context)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{message.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling UserUnFollowedEvent: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", message.Id, Program.AppName, message);

                var command = new UnFollowCommand { FollowerId = message.FollowerId, FollowedUserId = message.FollowedUserId };

                await _mediator.Send(command);
            }
        }
    }
}
