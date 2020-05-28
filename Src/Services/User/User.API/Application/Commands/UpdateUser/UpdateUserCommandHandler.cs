using Arise.DDD.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using Photography.Services.User.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.UpdateUser
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UpdateUserCommandHandler> _logger;

        public UpdateUserCommandHandler(IUserRepository userRepository, ILogger<UpdateUserCommandHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);

            if (user == null)
                throw new DomainException("用户不存在。");

            if (user.Nickname.ToLower() != request.Nickname.ToLower())
                throw new DomainException("昵称已存在。");

            user.Update(request.Nickname, request.Gender, request.Birthday, request.UserType, request.Province, request.City, request.Sign);

            _userRepository.Update(user);

            return await _userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
