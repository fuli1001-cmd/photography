﻿using ApplicationMessages.Events.Order;
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
    public class OrderFinishedEventHandler : IHandleMessages<OrderFinishedEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<OrderFinishedEventHandler> _logger;

        public OrderFinishedEventHandler(IUserRepository userRepository, ILogger<OrderFinishedEventHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(OrderFinishedEvent message, IMessageHandlerContext context)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{message.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling OrderFinishedEvent: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", message.Id, Program.AppName, message);

                // 减少用户1的出片阶段订单数量
                var user1 = await _userRepository.GetByIdAsync(message.User1Id);
                //user1.DecreaseProductionStageOrderCount();
                user1.DecreaseOngoingOrderCount();

                // 减少用户2的出片阶段订单数量
                var user2 = await _userRepository.GetByIdAsync(message.User2Id);
                //user2.DecreaseProductionStageOrderCount();
                user2.DecreaseOngoingOrderCount();

                await _userRepository.UnitOfWork.SaveEntitiesAsync();
            }
        }
    }
}
