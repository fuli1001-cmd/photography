using ApplicationMessages.Events.User;
using Arise.DDD.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.User.API.Query.Interfaces;
using Photography.Services.User.API.Query.ViewModels;
using Photography.Services.User.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.User.SetOrgAuthStatus
{
    public class SetOrgAuthStatusCommandHandler : IRequestHandler<SetOrgAuthStatusCommand, UserOrgAuthInfo>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserQueries _userQueries;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<SetOrgAuthStatusCommandHandler> _logger;
        private readonly IServiceProvider _serviceProvider;

        private IMessageSession _messageSession;

        public SetOrgAuthStatusCommandHandler(IUserRepository userRepository, IUserQueries userQueries,
            IServiceProvider serviceProvider, IHttpContextAccessor httpContextAccessor, ILogger<SetOrgAuthStatusCommandHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _userQueries = userQueries ?? throw new ArgumentNullException(nameof(userQueries));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<UserOrgAuthInfo> Handle(SetOrgAuthStatusCommand request, CancellationToken cancellationToken)
        {
            Guid userId;

            var role = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
            if (role == "admin")
            {
                if (request.UserId == null)
                    throw new ClientException("操作失败", new List<string> { $"UserId is need." });

                userId = request.UserId.Value;
            }
            else
            {
                userId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

                if (request.Status != IdAuthStatus.NoIdCard)
                    throw new ClientException("操作失败", new List<string> { $"Only admin can set user org auth status to {request.Status}" });
            }

            var user = await _userRepository.GetByIdAsync(userId);
            user.SetOrgAuthStatus(request.Status);

            if (await _userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
                await SendUserOrgAuthStatusChangedEventAsync(userId, request.Status);

            return await _userQueries.GetUserOrgAuthInfoAsync(userId);
        }

        private async Task SendUserOrgAuthStatusChangedEventAsync(Guid userId, IdAuthStatus status)
        {
            var @event = new UserOrgAuthStatusChangedEvent { UserId = userId, Status = (int)status };
            _messageSession = (IMessageSession)_serviceProvider.GetService(typeof(IMessageSession));
            await _messageSession.Publish(@event);
            _logger.LogInformation("----- Published UserOrgAuthStatusChangedEvent: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);
        }
    }
}
