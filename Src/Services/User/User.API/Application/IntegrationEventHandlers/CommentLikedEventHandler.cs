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
    public class CommentLikedEventHandler : IHandleMessages<CommentLikedEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<CommentLikedEventHandler> _logger;

        public CommentLikedEventHandler(IUserRepository userRepository, ILogger<CommentLikedEventHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(CommentLikedEvent message, IMessageHandlerContext context)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{message.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling CommentLikedEvent: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", message.Id, Program.AppName, message);

                var postUser = await _userRepository.GetByIdAsync(message.PostUserId);
                postUser.IncreaseLikedCount();

                await _userRepository.UnitOfWork.SaveEntitiesAsync();
            }
        }
    }
}
