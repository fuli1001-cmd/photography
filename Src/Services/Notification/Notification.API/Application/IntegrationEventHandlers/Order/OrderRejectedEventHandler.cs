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
    public class OrderRejectedEventHandler : IHandleMessages<OrderRejectedEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OrderRejectedEventHandler> _logger;

        public OrderRejectedEventHandler(IMediator mediator, ILogger<OrderRejectedEventHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(OrderRejectedEvent message, IMessageHandlerContext context)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{message.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling OrderRejectedEvent: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", message.Id, Program.AppName, message);

                // 创建订单已拒绝的事件
                var createEventCommand = new CreateEventCommand
                {
                    FromUserId = message.ProcessingUserId,
                    ToUserId = message.AnotherUserId,
                    EventType = Domain.AggregatesModel.EventAggregate.EventType.RejectOrder,
                    CommentText = message.Description,
                    OrderId = message.OrderId,
                    PushMessage = "你的约拍请求已被对方拒绝"
                };

                await _mediator.Send(createEventCommand);
            }
        }
    }
}
