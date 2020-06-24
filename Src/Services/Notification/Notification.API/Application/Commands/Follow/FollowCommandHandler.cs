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
            _userRelationRepository = userRelationRepository;
            _logger = logger;
        }

        public async Task<bool> Handle(FollowCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("****************** FollowCommandHandler 1");

            var ur = await _userRelationRepository.GetUserRelationAsync(request.FollowerId, request.FollowedUserId);

            _logger.LogInformation("****************** FollowCommandHandler 2");

            if (ur == null)
            {
                _logger.LogInformation("****************** FollowCommandHandler 3");
                ur = new UserRelation(request.FollowerId, request.FollowedUserId);
                _logger.LogInformation("****************** FollowCommandHandler 4");
                _userRelationRepository.Add(ur);
                _logger.LogInformation("****************** FollowCommandHandler 5");
                var result = await _userRelationRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
                _logger.LogInformation("****************** FollowCommandHandler 6 result: {result}", result);
            }

            _logger.LogInformation("****************** FollowCommandHandler 7");

            return true;
        }
    }
}
