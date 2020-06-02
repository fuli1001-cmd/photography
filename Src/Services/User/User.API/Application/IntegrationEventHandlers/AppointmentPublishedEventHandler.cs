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
    public class AppointmentPublishedEventHandler : IHandleMessages<AppointmentPublishedEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AppointmentPublishedEventHandler> _logger;

        public AppointmentPublishedEventHandler(IUserRepository userRepository, ILogger<AppointmentPublishedEventHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(AppointmentPublishedEvent message, IMessageHandlerContext context)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{message.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling AppointmentPublishedEvent: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", message.Id, Program.AppName, message);

                var user = await _userRepository.GetByIdAsync(message.UserId);
                user.IncreaseAppointmentCount();
                await _userRepository.UnitOfWork.SaveEntitiesAsync();
            }
        }
    }
}
