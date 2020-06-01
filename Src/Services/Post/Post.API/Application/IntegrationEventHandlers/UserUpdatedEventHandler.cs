using ApplicationMessages.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.Post.API.Application.Commands.User.UpdateUser;
using Photography.Services.Post.Domain.AggregatesModel.UserAggregate;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.IntegrationEventHandlers
{
    public class UserUpdatedEventHandler : IHandleMessages<UserUpdatedEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UserUpdatedEventHandler> _logger;

        public UserUpdatedEventHandler(IMediator mediator, ILogger<UserUpdatedEventHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(UserUpdatedEvent message, IMessageHandlerContext context)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{message.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling UserUpdatedEvent: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", message.Id, Program.AppName, message);

                var command = new UpdateUserCommand { UserId = message.UserId, Avatar = message.Avatar, NickName = message.NickName, UserType = (UserType?)message.UserType };

                await _mediator.Send(command);
            }
        }
    }
}
