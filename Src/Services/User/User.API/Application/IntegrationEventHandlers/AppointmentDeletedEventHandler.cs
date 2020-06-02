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
    public class AppointmentDeletedEventHandler : IHandleMessages<AppointmentDeletedEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AppointmentDeletedEventHandler> _logger;

        public AppointmentDeletedEventHandler(IUserRepository userRepository, ILogger<AppointmentDeletedEventHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(AppointmentDeletedEvent message, IMessageHandlerContext context)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{message.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling AppointmentDeletedEvent: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", message.Id, Program.AppName, message);

                var user = await _userRepository.GetByIdAsync(message.UserId);
                user.DecreaseAppointmentCount();
                await _userRepository.UnitOfWork.SaveEntitiesAsync();
            }
        }
    }
}
