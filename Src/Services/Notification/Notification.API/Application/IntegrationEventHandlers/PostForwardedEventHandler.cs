using ApplicationMessages.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.Notification.API.Application.Commands.CreateEvent;
using Photography.Services.Notification.API.Application.Commands.CreatePost;
using Photography.Services.Notification.Domain.AggregatesModel.EventAggregate;
using Photography.Services.Notification.Domain.AggregatesModel.PostAggregate;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Notification.API.Application.IntegrationEventHandlers
{
    public class PostForwardedEventHandler : IHandleMessages<PostForwardedEvent>
    {
        private readonly IMediator _mediator;
        private readonly IPostRepository _postRepository;
        private readonly ILogger<PostForwardedEventHandler> _logger;

        public PostForwardedEventHandler(IMediator mediator, IPostRepository postRepository, ILogger<PostForwardedEventHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(PostForwardedEvent message, IMessageHandlerContext context)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{message.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling PostForwardedEvent: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", message.Id, Program.AppName, message);

                foreach(var info in message.ForwardInfos)
                {
                    // 创建新帖子，由于是转发的帖子，其图片为原帖子的图片
                    var originalPost = await _postRepository.GetByIdAsync(info.OriginalPostId);
                    var createPostCommand = new CreatePostCommand
                    {
                        PostId = info.NewPostId,
                        Image = originalPost.Image
                    };
                    await _mediator.Send(createPostCommand);

                    // 创建事件
                    var createEventCommand = new CreateEventCommand
                    {
                        FromUserId = info.ForwardUserId,
                        ToUserId = info.OriginalPostUserId,
                        PostId = info.OriginalPostId,
                        EventType = EventType.ForwardPost
                    };
                    await _mediator.Send(createEventCommand);
                }
            }
        }
    }
}
