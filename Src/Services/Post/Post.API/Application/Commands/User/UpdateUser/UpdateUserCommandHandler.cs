using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Photography.Services.Post.API.Settings;
using Photography.Services.Post.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.User.UpdateUser
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly PostScoreRewardSettings _scoreRewardSettings;
        private readonly ILogger<UpdateUserCommandHandler> _logger;

        public UpdateUserCommandHandler(IUserRepository userRepository,
            IOptionsSnapshot<PostScoreRewardSettings> scoreRewardOptions, 
            ILogger<UpdateUserCommandHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _scoreRewardSettings = scoreRewardOptions?.Value ?? throw new ArgumentNullException(nameof(scoreRewardOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);

            if (user == null)
            {
                _logger.LogError("user {UserId} doesn't exist.", request.UserId);
                return false;
            }

            user.Update(request.NickName, request.Avatar, request.UserType, _scoreRewardSettings.SetUserProperty);

            return await _userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
