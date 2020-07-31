﻿using ApplicationMessages.Events;
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
    public class AppointmentDealCreatedEventHandler : IHandleMessages<AppointmentDealCreatedEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AppointmentDealCreatedEventHandler> _logger;

        public AppointmentDealCreatedEventHandler(IUserRepository userRepository, ILogger<AppointmentDealCreatedEventHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(AppointmentDealCreatedEvent message, IMessageHandlerContext context)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{message.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling AppointmentDealCreatedEvent: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", message.Id, Program.AppName, message);

                // 增加用户1待确认订单数量
                var user1 = await _userRepository.GetByIdAsync(message.User1Id);
                user1.IncreaseWaitingForConfirmOrderCount();

                // 增加用户2待确认订单数量
                var user2 = await _userRepository.GetByIdAsync(message.User2Id);
                user2.IncreaseWaitingForConfirmOrderCount();

                await _userRepository.UnitOfWork.SaveEntitiesAsync();
            }
        }
    }
}
