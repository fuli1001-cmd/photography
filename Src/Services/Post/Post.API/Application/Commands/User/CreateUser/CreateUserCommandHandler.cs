using MediatR;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.User.CreateUser
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
            _logger.LogInformation("********************Handle CreateUserCommand******************");
            _logger.LogInformation("{@CreateUserCommand}", request);
            var user = new Domain.AggregatesModel.UserAggregate.User(request.Id);
            _userRepository.Add(user);
            return await _userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
