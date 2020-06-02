using ApplicationMessages.Events;
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
    public class OrderCanceledEventHandler : IHandleMessages<OrderCanceledEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<OrderCanceledEventHandler> _logger;

        public OrderCanceledEventHandler(IUserRepository userRepository, ILogger<OrderCanceledEventHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(OrderCanceledEvent message, IMessageHandlerContext context)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{message.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling OrderCanceledEvent: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", message.Id, Program.AppName, message);

                var processingUser = await _userRepository.GetByIdAsync(message.ProcessingUserId);
                processingUser.DecreaseOngoingOrderCount();

                var anotherUser = await _userRepository.GetByIdAsync(message.AnotherUserId);
                anotherUser.DecreaseOngoingOrderCount();

                await _userRepository.UnitOfWork.SaveEntitiesAsync();
            }
        }
    }
}
