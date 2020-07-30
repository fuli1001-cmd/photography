using MediatR;
using Microsoft.Extensions.Logging;
using Photography.Services.Notification.Domain.AggregatesModel.UserRelationAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Notification.API.Application.Commands.Follow
{
    public class FollowCommandHandler : IRequestHandler<FollowCommand, bool>
    {
        private readonly IUserRelationRepository _userRelationRepository;
        private readonly ILogger<FollowCommandHandler> _logger;

        public FollowCommandHandler(IUserRelationRepository userRelationRepository,
            ILogger<FollowCommandHandler> logger)
        {
            _userRelationRepository = userRelationRepository ?? throw new ArgumentNullException(nameof(userRelationRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(FollowCommand request, CancellationToken cancellationToken)
        {
            var ur = await _userRelationRepository.GetUserRelationAsync(request.FollowerId, request.FollowedUserId);

            if (ur == null)
            {
                ur = new UserRelation(request.FollowerId, request.FollowedUserId);
                _userRelationRepository.Add(ur);
                var result = await _userRelationRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
            }

            return true;
        }
    }
}
