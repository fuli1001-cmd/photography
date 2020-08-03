using Arise.DDD.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using Photography.Services.Notification.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Notification.API.Application.Commands.UpdatePushInfo
{
    public class UpdatePushInfoCommandHandler : IRequestHandler<UpdatePushInfoCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UpdatePushInfoCommandHandler> _logger;

        public UpdatePushInfoCommandHandler(IUserRepository userRepository, ILogger<UpdatePushInfoCommandHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(UpdatePushInfoCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);

            if (user == null)
                throw new ClientException("操作失败", new List<string> { $"User {request.UserId} doesn't exist." });

            user.UpdatePushInfo(request.RegistrationId);

            return await _userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
