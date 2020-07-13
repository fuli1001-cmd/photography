using ApplicationMessages.Events;
using Arise.DDD.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.User.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.User.DisableUser
{
    public class DisableUserCommandHandler : IRequestHandler<DisableUserCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<DisableUserCommandHandler> _logger;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        private IMessageSession _messageSession;

        public DisableUserCommandHandler(IUserRepository userRepository,
            IConfiguration configuration,
            IServiceProvider serviceProvider, 
            ILogger<DisableUserCommandHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(DisableUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);

            if (user == null)
                throw new ClientException("操作失败", new List<string> { $"User {request.UserId} does not exist." });

            if (request.Disabled)
                user.Disable(_configuration.GetValue<double>("DisableHours", 0));
            else
                user.Enable();

            if (await _userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
            {
                await SendUserDisabledEventAsync(user);
                return true;
            }

            throw new ApplicationException("操作失败");
        }

        private async Task SendUserDisabledEventAsync(Domain.AggregatesModel.UserAggregate.User user)
        {
            var @event = new UserDisabledEvent { UserId = user.Id, DisabledTime = user.DisabledTime };
            _messageSession = (IMessageSession)_serviceProvider.GetService(typeof(IMessageSession));
            await _messageSession.Publish(@event);

            _logger.LogInformation("----- Published UserDisabledEvent: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);
        }
    }
}
