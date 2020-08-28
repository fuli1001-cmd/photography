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
    public class OrderAcceptedEventHandler : IHandleMessages<OrderAcceptedEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;
        private readonly ILogger<OrderAcceptedEventHandler> _logger;

        public OrderAcceptedEventHandler(IUserRepository userRepository, IMediator mediator, ILogger<OrderAcceptedEventHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(OrderAcceptedEvent message, IMessageHandlerContext context)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{message.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling OrderAcceptedEvent: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", message.Id, Program.AppName, message);

                var nickName = await _userRepository.GetNickNameAsync(message.UserId);
                
                // 创建订单已接受的事件
                var command = new CreateEventCommand
                {
                    FromUserId = message.UserId,
                    ToUserId = message.AnotherUserId,
                    EventType = Domain.AggregatesModel.EventAggregate.EventType.OrderAccepted,
                    CommentText = "已确认拍片",
                    OrderId = message.OrderId,
                    PushMessage = $"{nickName}已确认了您的约拍请求，请及时查看"
                };

                await _mediator.Send(command);
            }
        }
    }
}
