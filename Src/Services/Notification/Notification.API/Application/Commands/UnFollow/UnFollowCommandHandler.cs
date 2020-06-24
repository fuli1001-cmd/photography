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
            _logger.LogInformation("****************** UnFollowCommandHandler 1");

            var ur = await _userRelationRepository.GetUserRelationAsync(request.FollowerId, request.FollowedUserId);

            _logger.LogInformation("****************** UnFollowCommandHandler 2");

            if (ur != null)
            {
                _logger.LogInformation("****************** UnFollowCommandHandler 3");
                _userRelationRepository.Remove(ur);
                _logger.LogInformation("****************** UnFollowCommandHandler 4");
                var result = await _userRelationRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
                _logger.LogInformation("****************** UnFollowCommandHandler 5 result: {result}", result);
            }

            _logger.LogInformation("****************** UnFollowCommandHandler 6");

            return true;
        }
    }
}
