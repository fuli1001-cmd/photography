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
    public class PostForwardedEventHandler : IHandleMessages<PostForwardedEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<PostForwardedEventHandler> _logger;

        public PostForwardedEventHandler(IUserRepository userRepository, ILogger<PostForwardedEventHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(PostForwardedEvent message, IMessageHandlerContext context)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{message.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling PostForwardedEvent: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", message.Id, Program.AppName, message);

                var user = await _userRepository.GetByIdAsync(message.ForwardInfos[0].ForwardUserId);

                message.ForwardInfos.ForEach(info => user.IncreasePostCount());
                
                await _userRepository.UnitOfWork.SaveEntitiesAsync();
            }
        }
    }
}
