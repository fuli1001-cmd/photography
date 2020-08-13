using ApplicationMessages.Events.User;
using Arise.DDD.Messages.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.Order.API.Application.Commands.CreateUser;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Order.API.Application.IntegrationEventHandlers
{
    public class UserCreatedEventHandler : IHandleMessages<UserCreatedEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UserCreatedEventHandler> _logger;

        public UserCreatedEventHandler(IMediator mediator, ILogger<UserCreatedEventHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(UserCreatedEvent message, IMessageHandlerContext context)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{message.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling UserCreatedEvent: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", message.Id, Program.AppName, message);

                var command = new CreateUserCommand { UserId = message.UserId, NickName = message.NickName, ChatServerUserId = message.ChatServerUserId };

                await _mediator.Send(command);
            }
        }
    }
}
