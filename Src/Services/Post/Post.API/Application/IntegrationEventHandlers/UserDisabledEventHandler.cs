using ApplicationMessages.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.Post.API.Application.Commands.User.DisableUser;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.IntegrationEventHandlers
{
    public class UserDisabledEventHandler : IHandleMessages<UserDisabledEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UserDisabledEventHandler> _logger;

        public UserDisabledEventHandler(IMediator mediator, ILogger<UserDisabledEventHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(UserDisabledEvent message, IMessageHandlerContext context)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{message.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling UserDisabledEvent: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", message.Id, Program.AppName, message);

                var command = new DisableUserCommand { UserId = message.UserId, DisabledTime = message.DisabledTime };

                await _mediator.Send(command);
            }
        }
    }
}
