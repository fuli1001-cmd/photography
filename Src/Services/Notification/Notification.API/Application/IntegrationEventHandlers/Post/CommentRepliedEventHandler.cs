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
    public class CommentRepliedEventHandler : IHandleMessages<CommentRepliedEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;
        private readonly ILogger<CommentRepliedEventHandler> _logger;

        public CommentRepliedEventHandler(IUserRepository userRepository, IMediator mediator, ILogger<CommentRepliedEventHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(CommentRepliedEvent message, IMessageHandlerContext context)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{message.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling CommentRepliedEvent: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", message.Id, Program.AppName, message);

                var nickName = await _userRepository.GetNickNameAsync(message.FromUserId);

                var command = new CreateEventCommand
                {
                    FromUserId = message.FromUserId,
                    ToUserId = message.ToUserId,
                    PostId = message.PostId,
                    CommentId = message.CommentId,
                    CommentText = message.Text,
                    EventType = EventType.ReplyComment,
                    PushMessage = $"{nickName}回复了你的评论"
                };

                await _mediator.Send(command);
            }
        }
    }
}
