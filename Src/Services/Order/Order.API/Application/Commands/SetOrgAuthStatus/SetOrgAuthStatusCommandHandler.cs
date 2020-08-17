using ApplicationMessages.Events.User;
using Arise.DDD.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.Order.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Order.API.Application.Commands.User.SetOrgAuthStatus
{
    public class SetOrgAuthStatusCommandHandler : IRequestHandler<SetOrgAuthStatusCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<SetOrgAuthStatusCommandHandler> _logger;

        public SetOrgAuthStatusCommandHandler(IUserRepository userRepository, ILogger<SetOrgAuthStatusCommandHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(SetOrgAuthStatusCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            user.SetOrgAuthStatus(request.Status);
            return await _userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
