﻿using MediatR;
using Microsoft.Extensions.Logging;
using Photography.Services.Notification.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Notification.API.Application.Commands.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<CreateUserCommandHandler> _logger;

        public CreateUserCommandHandler(IUserRepository userRepository, ILogger<CreateUserCommandHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var nickName = "用户" + request.UserName;
            var user = new Domain.AggregatesModel.UserAggregate.User(request.Id, nickName);
            _userRepository.Add(user);
            return await _userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
