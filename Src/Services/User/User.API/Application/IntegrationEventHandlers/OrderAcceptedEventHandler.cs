using ApplicationMessages.Events.Order;
using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.User.Domain.AggregatesModel.UserAggregate;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.IntegrationEventHandlers
{
    public class OrderAcceptedEventHandler : IHandleMessages<OrderAcceptedEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<OrderAcceptedEventHandler> _logger;

        public OrderAcceptedEventHandler(IUserRepository userRepository, ILogger<OrderAcceptedEventHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(OrderAcceptedEvent message, IMessageHandlerContext context)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{message.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling OrderAcceptedEvent: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", message.Id, Program.AppName, message);

                // 增加用户1的进行中订单数量，并减少待确认订单数量
                var user1 = await _userRepository.GetByIdAsync(message.UserId);
                user1.IncreaseOngoingOrderCount();
                user1.DecreaseWaitingForConfirmOrderCount();

                // 增加用户2的进行中订单数量，并减少待确认订单数量
                var user2 = await _userRepository.GetByIdAsync(message.AnotherUserId);
                user2.IncreaseOngoingOrderCount();
                user2.DecreaseWaitingForConfirmOrderCount();

                await _userRepository.UnitOfWork.SaveEntitiesAsync();
            }
        }
    }
}
