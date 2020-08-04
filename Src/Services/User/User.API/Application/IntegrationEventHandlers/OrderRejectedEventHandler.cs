using ApplicationMessages.Events.Order;
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
    public class OrderRejectedEventHandler : IHandleMessages<OrderRejectedEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<OrderRejectedEventHandler> _logger;

        public OrderRejectedEventHandler(IUserRepository userRepository, ILogger<OrderRejectedEventHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(OrderRejectedEvent message, IMessageHandlerContext context)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{message.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling OrderRejectedEvent: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", message.Id, Program.AppName, message);

                // 减少用户1待确认订单数量
                var user1 = await _userRepository.GetByIdAsync(message.ProcessingUserId);
                user1.DecreaseWaitingForConfirmOrderCount();

                // 减少用户2待确认订单数量
                var user2 = await _userRepository.GetByIdAsync(message.AnotherUserId);
                user2.DecreaseWaitingForConfirmOrderCount();

                await _userRepository.UnitOfWork.SaveEntitiesAsync();
            }
        }
    }
}
