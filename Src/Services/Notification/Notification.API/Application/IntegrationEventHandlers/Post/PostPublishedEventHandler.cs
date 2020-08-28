using ApplicationMessages.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.Notification.API.Application.Commands.CreateEvent;
using Photography.Services.Notification.API.Application.Commands.CreatePost;
using Photography.Services.Notification.Domain.AggregatesModel.EventAggregate;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Notification.API.Application.IntegrationEventHandlers.Post
{
    public class PostPublishedEventHandler : IHandleMessages<PostPublishedEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PostPublishedEventHandler> _logger;

        public PostPublishedEventHandler(IMediator mediator, ILogger<PostPublishedEventHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(PostPublishedEvent message, IMessageHandlerContext context)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{message.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling PostPublishedEvent: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", message.Id, Program.AppName, message);

                #region 创建帖子
                var postCommand = new CreatePostCommand { PostId = message.PostId, Image = message.Image };
                await _mediator.Send(postCommand);
                #endregion

                #region 发布@用户通知
                foreach (var atUserId in message.AtUserIds)
                {
                    var eventCommand = new CreateEventCommand
                    {
                        FromUserId = message.UserId,
                        ToUserId = atUserId,
                        PostId = message.PostId,
                        EventType = EventType.AtUserInPost,
                        PushMessage = $"{message.Nickname}在作品中@了你"
                    };

                    await _mediator.Send(eventCommand);
                }
                #endregion
            }
        }
    }
}
