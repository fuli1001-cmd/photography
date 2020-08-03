using ApplicationMessages.Events;
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

namespace Photography.Services.Notification.API.Application.IntegrationEventHandlers
{
    public class AppliedJoinCircleEventHandler : IHandleMessages<AppliedJoinCircleEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;
        private readonly ILogger<AppliedJoinCircleEventHandler> _logger;

        public AppliedJoinCircleEventHandler(IUserRepository userRepository, IMediator mediator, ILogger<AppliedJoinCircleEventHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(AppliedJoinCircleEvent message, IMessageHandlerContext context)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{message.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling AppliedJoinCircleEvent: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", message.Id, Program.AppName, message);

                var fromUser = await _userRepository.GetByIdAsync(message.ApplyUserId);

                var command = new CreateEventCommand
                {
                    FromUserId = message.ApplyUserId,
                    ToUserId = message.CircleOwnerId,
                    CircleId = message.CircleId,
                    CircleName = message.CircleName,
                    CommentText = message.ApplyDescription, // CommentText创建时还没有圈子功能，这里共用CommentText来存储加圈描述
                    EventType = Domain.AggregatesModel.EventAggregate.EventType.ApplyJoinCircle,
                    PushMessage = $"{fromUser.Nickname}申请加入圈子{message.CircleName}"
                };

                await _mediator.Send(command);
            }
        }
    }
}
