﻿using ApplicationMessages.Events.Post;
using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.Notification.API.Application.Commands.CreateEvent;
using Photography.Services.Notification.Domain.AggregatesModel.EventAggregate;
using Photography.Services.Notification.Domain.AggregatesModel.UserAggregate;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Notification.API.Application.IntegrationEventHandlers
{
    public class UserAtedEventHandler : IHandleMessages<UserAtedEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;
        private readonly ILogger<UserAtedEventHandler> _logger;

        public UserAtedEventHandler(IUserRepository userRepository, IMediator mediator, ILogger<UserAtedEventHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(UserAtedEvent message, IMessageHandlerContext context)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{message.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling UserAtedEvent: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", message.Id, Program.AppName, message);

                var user = await _userRepository.GetByIdAsync(message.PostUserId);

                foreach (var atUserId in message.AtUserIds)
                {
                    var eventCommand = new CreateEventCommand
                    {
                        FromUserId = message.PostUserId,
                        ToUserId = atUserId,
                        PostId = message.PostId,
                        EventType = EventType.AtUserInPost,
                        PushMessage = $"{user.Nickname}在作品中@了你"
                    };

                    await _mediator.Send(eventCommand);
                }
            }
        }
    }
}
