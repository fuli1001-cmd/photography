using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Photography.Services.User.Domain.AggregatesModel.UserAggregate;
using Photography.Services.User.Domain.AggregatesModel.UserRelationAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.ToggleFollow
{
    public class ToggleFollowCommandHandler : IRequestHandler<ToggleFollowCommand, bool>
    {
        private readonly IUserRelationRepository _userRelationRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ToggleFollowCommandHandler> _logger;

        public ToggleFollowCommandHandler(IUserRelationRepository userRelationRepository, IHttpContextAccessor httpContextAccessor, ILogger<ToggleFollowCommandHandler> logger)
        {
            _userRelationRepository = userRelationRepository ?? throw new ArgumentNullException(nameof(userRelationRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ToggleFollowCommand request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var ur = await _userRelationRepository.GetAsync(userId, request.UserIdToFollow);
            if (ur == null)
            {
                ur = new UserRelation(userId, request.UserIdToFollow);
                _userRelationRepository.Add(ur);
            }
            else
            {
                _userRelationRepository.Remove(ur);
            }
            return await _userRelationRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
