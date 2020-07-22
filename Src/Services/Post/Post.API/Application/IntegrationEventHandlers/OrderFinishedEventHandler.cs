using ApplicationMessages.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NServiceBus;
using Photography.Services.Post.API.Settings;
using Photography.Services.Post.Domain.AggregatesModel.UserAggregate;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.IntegrationEventHandlers
{
    public class OrderFinishedEventHandler : IHandleMessages<OrderFinishedEvent>
    {
        private readonly AppointmentSettings _appointmentSettings;
        private readonly ILogger<OrderFinishedEventHandler> _logger;
        private readonly IServiceProvider _serviceProvider;

        private IMessageSession _messageSession;

        public OrderFinishedEventHandler(
            IOptionsSnapshot<AppointmentSettings> appointmentOptions,
            IServiceProvider serviceProvider,
            ILogger<OrderFinishedEventHandler> logger)
        {
            _appointmentSettings = appointmentOptions?.Value ?? throw new ArgumentNullException(nameof(appointmentOptions));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(OrderFinishedEvent message, IMessageHandlerContext context)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{message.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling OrderFinishedEvent: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", message.Id, Program.AppName, message);

                var eventTasks = new List<Task>
                {
                    SendAppointmentScoreChangedEventAsync(message.User1Id),
                    SendAppointmentScoreChangedEventAsync(message.User2Id)
                };
                await Task.WhenAll(eventTasks);
            }
        }

        private async Task SendAppointmentScoreChangedEventAsync(Guid userId)
        {
            var @event = new AppointmentScoreChangedEvent { UserId = userId, ChangedScore = _appointmentSettings.FinishDealScore };
            _messageSession = (IMessageSession)_serviceProvider.GetService(typeof(IMessageSession));
            await _messageSession.Publish(@event);

            _logger.LogInformation("----- Published AppointmentScoreChangedEvent: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);
        }
    }
}
