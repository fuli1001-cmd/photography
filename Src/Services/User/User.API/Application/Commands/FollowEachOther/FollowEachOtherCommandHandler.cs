using MediatR;
using Microsoft.Extensions.Logging;
using Photography.Services.User.Domain.AggregatesModel.UserAggregate;
using Photography.Services.User.Domain.AggregatesModel.UserRelationAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.FollowEachOther
{
    public class FollowEachOtherCommandHandler : IRequestHandler<FollowEachOtherCommand, bool>
    {
        private readonly IUserRelationRepository _userRelationRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<FollowEachOtherCommandHandler> _logger;

        public FollowEachOtherCommandHandler(IUserRelationRepository userRelationRepository,
            IUserRepository userRepository,
            ILogger<FollowEachOtherCommandHandler> logger)
        {
            _userRelationRepository = userRelationRepository ?? throw new ArgumentNullException(nameof(userRelationRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(FollowEachOtherCommand request, CancellationToken cancellationToken)
        {
            var anotherUserId = await _userRepository.GetUserIdByCodeAsync(request.InvitingUserCode);

            if (anotherUserId != null)
            {
                var userId = Guid.Parse(request.UserId);
                var relation1 = new UserRelation(userId, anotherUserId.Value);
                var relation2 = new UserRelation(anotherUserId.Value, userId);

                _userRelationRepository.Add(relation1);
                _userRelationRepository.Add(relation2);

                return await _userRelationRepository.UnitOfWork.SaveEntitiesAsync();
            }

            return false;
        }
    }
}
