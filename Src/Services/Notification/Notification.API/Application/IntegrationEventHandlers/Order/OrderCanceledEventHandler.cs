using ApplicationMessages;
using ApplicationMessages.Events.Order;
using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.Notification.API.Application.Commands.CreateEvent;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Notification.API.Application.IntegrationEventHandlers.Order
{
    public class OrderCanceledEventHandler : IHandleMessages<OrderCanceledEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OrderCanceledEventHandler> _logger;

        public OrderCanceledEventHandler(IMediator mediator, ILogger<OrderCanceledEventHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(OrderCanceledEvent message, IMessageHandlerContext context)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{message.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling OrderCanceledEvent: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", message.Id, Program.AppName, message);

                // 创建订单已取消的事件
                var createEventCommand = new CreateEventCommand
                {
                    FromUserId = message.ProcessingUserId,
                    ToUserId = message.AnotherUserId,
                    EventType = Domain.AggregatesModel.EventAggregate.EventType.CancelOrder,
                    CommentText = message.Description,
                    OrderId = message.OrderId,
                    PushMessage = "你收到的约拍请求已被对方取消"
                };

                await _mediator.Send(createEventCommand);
            }
        }
    }
}
