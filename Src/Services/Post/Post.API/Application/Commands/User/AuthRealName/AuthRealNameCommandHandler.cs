using ApplicationMessages.Events;
using Arise.DDD.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.Post.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.User.AuthRealName
{
    public class AuthRealNameCommandHandler : IRequestHandler<AuthRealNameCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AuthRealNameCommandHandler> _logger;

        public AuthRealNameCommandHandler(IUserRepository userRepository, ILogger<AuthRealNameCommandHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(AuthRealNameCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);

            if (user == null)
                throw new ClientException("操作失败", new List<string> { $"User {request.UserId} does not exist." });

            user.AuthRealName(request.Passed);

            return await _userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
