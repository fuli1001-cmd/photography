using Arise.DDD.Messages.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.Post.API.Application.Commands.User.CreateUser;
using Photography.Services.Post.API.Application.Commands.User.FollowEachOther;
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

                var createUserCommand = new CreateUserCommand { UserId = message.Id, UserName = message.UserName };

                if (await _mediator.Send(createUserCommand) && !string.IsNullOrEmpty(message.InvitingUserCode))
                {
                    // 如果有推荐人的邀请码，则建立相互关注的关系
                    var followEachOtherCommand = new FollowEachOtherCommand
                    {
                        UserId = message.Id,
                        InvitingUserCode = message.InvitingUserCode
                    };
                    await _mediator.Send(followEachOtherCommand);
                }
            }
        }
    }
}
