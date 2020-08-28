using ApplicationMessages.Events.Order;
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

namespace Photography.Services.Notification.API.Application.IntegrationEventHandlers.Order
{
    public class OriginalPhotoSelectedEventHandler : IHandleMessages<OriginalPhotoSelectedEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;
        private readonly ILogger<OriginalPhotoSelectedEventHandler> _logger;

        public OriginalPhotoSelectedEventHandler(IUserRepository userRepository, IMediator mediator, ILogger<OriginalPhotoSelectedEventHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(OriginalPhotoSelectedEvent message, IMessageHandlerContext context)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{message.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling OriginalPhotoSelectedEvent: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", message.Id, Program.AppName, message);

                var nickName = await _userRepository.GetNickNameAsync(message.SelectPhotoUserId);

                // 创建订单已接受的事件
                var command = new CreateEventCommand
                {
                    FromUserId = message.SelectPhotoUserId,
                    ToUserId = message.AnotherUserId,
                    EventType = Domain.AggregatesModel.EventAggregate.EventType.OriginalPhotoSelected,
                    CommentText = "已选片",
                    OrderId = message.OrderId,
                    PushMessage = $"{nickName}已选片"
                };

                await _mediator.Send(command);
            }
        }
    }
}
