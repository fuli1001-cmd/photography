using ApplicationMessages.Events;
using Arise.DDD.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.User.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.User.AuthRealName
{
    public class AuthRealNameCommandHandler : IRequestHandler<AuthRealNameCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuthRealNameCommandHandler> _logger;
        private readonly IServiceProvider _serviceProvider;

        private IMessageSession _messageSession;

        public AuthRealNameCommandHandler(
            IUserRepository userRepository,
            IHttpContextAccessor httpContextAccessor,
            IServiceProvider serviceProvider, 
            ILogger<AuthRealNameCommandHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(AuthRealNameCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);

            if (user == null)
                throw new ClientException("操作失败", new List<string> { $"User {request.UserId} does not exist." });

            user.AuthRealName(request.Passed);

            if (await _userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
            {
                await SendIdAuthenticatedEventAsync(request.UserId, request.Passed);
                return true;
            }

            throw new ApplicationException("操作失败");
        }

        private async Task SendIdAuthenticatedEventAsync(Guid userId, bool passed)
        {
            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var @event = new IdAuthenticatedEvent { UserId = userId, Passed = passed, OperatorId = myId };
            _messageSession = (IMessageSession)_serviceProvider.GetService(typeof(IMessageSession));
            await _messageSession.Publish(@event);

            _logger.LogInformation("----- Published IdAuthenticatedEvent: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);
        }
    }
}
