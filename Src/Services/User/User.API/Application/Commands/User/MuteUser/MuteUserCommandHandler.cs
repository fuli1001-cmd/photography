using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Photography.Services.User.Domain.AggregatesModel.UserRelationAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.User.MuteUser
{
    public class MuteUserCommandHandler : IRequestHandler<MuteUserCommand, bool>
    {
        private readonly IUserRelationRepository _userRelationRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<MuteUserCommandHandler> _logger;

        public MuteUserCommandHandler(
            IUserRelationRepository userRelationRepository,
            IHttpContextAccessor httpContextAccessor,
            ILogger<MuteUserCommandHandler> logger)
        {
            _userRelationRepository = userRelationRepository ?? throw new ArgumentNullException(nameof(userRelationRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(MuteUserCommand request, CancellationToken cancellationToken)
        {
            var claim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            var myId = claim == null ? Guid.Empty : Guid.Parse(claim.Value);

            var userRelation = await _userRelationRepository.GetAsync(myId, request.UserId);

            if (request.Muted)
            {
                if (userRelation == null)
                {
                    userRelation = new UserRelation(myId, request.UserId);
                    _userRelationRepository.Add(userRelation);
                }

                userRelation.Mute();
            }
            else if (userRelation != null)
                userRelation.UnMute();

            return await _userRelationRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
