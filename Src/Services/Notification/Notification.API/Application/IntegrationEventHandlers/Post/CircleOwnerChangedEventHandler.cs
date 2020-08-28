using ApplicationMessages.Events.Post;
using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.Notification.API.Application.Commands.CreateEvent;
using Photography.Services.Notification.Domain.AggregatesModel.UserAggregate;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Notification.API.Application.IntegrationEventHandlers.Post
{
    public class CircleOwnerChangedEventHandler : IHandleMessages<CircleOwnerChangedEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;
        private readonly ILogger<CircleOwnerChangedEventHandler> _logger;

        public CircleOwnerChangedEventHandler(IUserRepository userRepository, IMediator mediator, ILogger<CircleOwnerChangedEventHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(CircleOwnerChangedEvent message, IMessageHandlerContext context)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{message.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling CircleOwnerChangedEvent: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", message.Id, Program.AppName, message);

                var nickName = await _userRepository.GetNickNameAsync(message.OldOwnerId);

                var command = new CreateEventCommand
                {
                    FromUserId = message.OldOwnerId,
                    ToUserId = message.NewOwnerId,
                    CircleId = message.CircleId,
                    CircleName = message.CircleName,
                    EventType = Domain.AggregatesModel.EventAggregate.EventType.CircleOwnerChanged,
                    PushMessage = $"{nickName}将圈子{message.CircleName}转让给了你"
                };

                await _mediator.Send(command);
            }
        }
    }
}
