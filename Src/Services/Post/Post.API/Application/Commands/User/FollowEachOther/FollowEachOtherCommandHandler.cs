using MediatR;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.Domain.AggregatesModel.UserAggregate;
using Photography.Services.Post.Domain.AggregatesModel.UserRelationAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.User.FollowEachOther
{
    public class FollowEachOtherCommandHandler : IRequestHandler<FollowEachOtherCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserRelationRepository _userRelationRepository;
        private readonly ILogger<FollowEachOtherCommandHandler> _logger;

        public FollowEachOtherCommandHandler(IUserRepository userRepository,
            IUserRelationRepository userRelationRepository,
            ILogger<FollowEachOtherCommandHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _userRelationRepository = userRelationRepository ?? throw new ArgumentNullException(nameof(userRelationRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(FollowEachOtherCommand request, CancellationToken cancellationToken)
        {
            //var anotherUserId = await _userRepository.GetUserIdByCodeAsync(request.InvitingUserCode);

            //if (anotherUserId != null)
            //{
            //    var userId = Guid.Parse(request.UserId);
            //    var relation1 = new UserRelation(userId, anotherUserId.Value);
            //    var relation2 = new UserRelation(anotherUserId.Value, userId);

            //    _userRelationRepository.Add(relation1);
            //    _userRelationRepository.Add(relation2);

            //    return await _userRelationRepository.UnitOfWork.SaveEntitiesAsync();
            //}

            return false;
        }
    }
}
