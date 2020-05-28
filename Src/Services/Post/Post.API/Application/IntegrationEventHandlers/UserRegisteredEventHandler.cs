using Arise.DDD.Messages.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.Post.API.Application.Commands.User.CreateUser;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.IntegrationEventHandlers
{
    public class UserRegisteredEventHandler : IHandleMessages<UserRegisteredEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UserRegisteredEventHandler> _logger;

        public UserRegisteredEventHandler(IMediator mediator, ILogger<UserRegisteredEventHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(UserRegisteredEvent message, IMessageHandlerContext context)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{message.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling UserRegisteredEvent: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", message.Id, Program.AppName, message);

                var command = new CreateUserCommand { Id = message.Id };

                try
                {
                    await _mediator.Send(command);
                }
                catch(Exception ex)
                {
                    _logger.LogError("{err}", ex.Message);
                    if (ex.InnerException != null)
                        _logger.LogError("{err2}", ex.InnerException.Message);
                }
            }
        }
    }
}
