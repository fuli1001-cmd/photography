using ApplicationMessages.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.Notification.API.Application.Commands.CreatePost;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Notification.API.Application.IntegrationEventHandlers
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

                var command = new CreatePostCommand { PostId = message.PostId, Image = message.Image };

                await _mediator.Send(command);
            }
        }
    }
}
