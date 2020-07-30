using MediatR;
using Microsoft.Extensions.Logging;
using Photography.Services.Notification.Domain.AggregatesModel.UserRelationAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Notification.API.Application.Commands.UnFollow
{
    public class UnFollowCommandHandler : IRequestHandler<UnFollowCommand, bool>
    {
        private readonly IUserRelationRepository _userRelationRepository;
        private readonly ILogger<UnFollowCommandHandler> _logger;

        public UnFollowCommandHandler(IUserRelationRepository userRelationRepository,
            ILogger<UnFollowCommandHandler> logger)
        {
            _userRelationRepository = userRelationRepository;
            _logger = logger;
        }

        public async Task<bool> Handle(UnFollowCommand request, CancellationToken cancellationToken)
        {
            var ur = await _userRelationRepository.GetUserRelationAsync(request.FollowerId, request.FollowedUserId);

            if (ur != null)
            {
                _userRelationRepository.Remove(ur);
                var result = await _userRelationRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
            }

            return true;
        }
    }
}
