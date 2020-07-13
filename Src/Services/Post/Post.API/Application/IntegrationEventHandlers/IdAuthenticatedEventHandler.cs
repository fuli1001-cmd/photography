using ApplicationMessages.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.Post.API.Application.Commands.User.AuthRealName;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.IntegrationEventHandlers
{
    public class IdAuthenticatedEventHandler : IHandleMessages<IdAuthenticatedEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<IdAuthenticatedEventHandler> _logger;

        public IdAuthenticatedEventHandler(IMediator mediator, ILogger<IdAuthenticatedEventHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(IdAuthenticatedEvent message, IMessageHandlerContext context)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{message.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling IdAuthenticatedEvent: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", message.Id, Program.AppName, message);

                var command = new AuthRealNameCommand { UserId = message.UserId, Passed = message.Passed };

                await _mediator.Send(command);
            }
        }
    }
}
