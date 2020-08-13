using ApplicationMessages.Events.User;
using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.User.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.User.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<CreateUserCommandHandler> _logger;
        private readonly IServiceProvider _serviceProvider;

        private IMessageSession _messageSession;

        public CreateUserCommandHandler(IUserRepository userRepository, IServiceProvider serviceProvider, ILogger<CreateUserCommandHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var nickName = "用户" + request.UserName;
            var user = new Domain.AggregatesModel.UserAggregate.User(request.Id, request.UserName, request.PhoneNumber, request.Code, nickName);
            _userRepository.Add(user);

            if (await _userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
            {
                await SendUserCreatedEventAsync(user);
                return true;
            }

            throw new ApplicationException($"{Program.AppName}: 保存用户失败");
        }

        private async Task SendUserCreatedEventAsync(Domain.AggregatesModel.UserAggregate.User user)
        {
            var @event = new UserCreatedEvent { UserId = user.Id, NickName = user.Nickname, ChatServerUserId = user.ChatServerUserId };
            _messageSession = (IMessageSession)_serviceProvider.GetService(typeof(IMessageSession));
            await _messageSession.Publish(@event);
            _logger.LogInformation("----- Published UserCreatedEvent: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);
        }
    }
}
