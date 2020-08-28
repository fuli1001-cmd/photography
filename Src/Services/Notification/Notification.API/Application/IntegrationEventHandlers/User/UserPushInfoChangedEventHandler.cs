using ApplicationMessages.Events.User;
using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.Notification.API.Application.Commands.UpdatePushInfo;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Notification.API.Application.IntegrationEventHandlers.User
{
    public class UserPushInfoChangedEventHandler : IHandleMessages<UserPushInfoChangedEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UserPushInfoChangedEventHandler> _logger;

        public UserPushInfoChangedEventHandler(IMediator mediator, ILogger<UserPushInfoChangedEventHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(UserPushInfoChangedEvent message, IMessageHandlerContext context)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{message.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling UserPushInfoChangedEvent: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", message.Id, Program.AppName, message);

                var command = new UpdatePushInfoCommand { UserId = message.UserId, RegistrationId = message.RegistrationId };

                await _mediator.Send(command);
            }
        }
    }
}
