using MediatR;
using Microsoft.Extensions.Logging;
using Photography.Services.Notification.Domain.AggregatesModel.UserRelationAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Notification.API.Application.Commands.User.Follow
{
    public class FollowCommandHandler : IRequestHandler<FollowCommand, bool>
    {
        private readonly IUserRelationRepository _userRelationRepository;
        private readonly ILogger<FollowCommandHandler> _logger;

        public FollowCommandHandler(IUserRelationRepository userRelationRepository,
            ILogger<FollowCommandHandler> logger)
        {
            _userRelationRepository = userRelationRepository;
            _logger = logger;
        }

        public async Task<bool> Handle(FollowCommand request, CancellationToken cancellationToken)
        {
            var ur = await _userRelationRepository.GetUserRelationAsync(request.FollowerId, request.FollowedUserId);
            
            if (ur == null)
            {
                ur = new UserRelation(request.FollowerId, request.FollowedUserId);
                _userRelationRepository.Add(ur);
                return await _userRelationRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
            }

            return true;
        }
    }
}
