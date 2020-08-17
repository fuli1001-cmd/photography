using ApplicationMessages.Events.User;
using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.Order.API.Application.Commands.User.SetOrgAuthStatus;
using Photography.Services.Order.Domain.AggregatesModel.UserAggregate;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Order.API.Application.IntegrationEventHandlers
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

                var command = new SetOrgAuthStatusCommand { UserId = message.UserId, Status = (AuthStatus)message.Status };

                await _mediator.Send(command);
            }
        }
    }
}
