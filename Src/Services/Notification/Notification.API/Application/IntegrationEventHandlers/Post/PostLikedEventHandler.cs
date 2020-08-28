using ApplicationMessages.Events;
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

namespace Photography.Services.Notification.API.Application.IntegrationEventHandlers.Post
{
    public class PostLikedEventHandler : IHandleMessages<PostLikedEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;
        private readonly ILogger<PostLikedEventHandler> _logger;

        public PostLikedEventHandler(IUserRepository userRepository, IMediator mediator, ILogger<PostLikedEventHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(PostLikedEvent message, IMessageHandlerContext context)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{message.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling PostLikedEvent: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", message.Id, Program.AppName, message);

                var nickName = await _userRepository.GetNickNameAsync(message.LikingUserId);

                var command = new CreateEventCommand
                {
                    FromUserId = message.LikingUserId,
                    ToUserId = message.PostUserId,
                    PostId = message.PostId,
                    EventType = EventType.LikePost,
                    PushMessage = $"{nickName}点赞了你的作品"
                };

                await _mediator.Send(command);
            }
        }
    }
}
