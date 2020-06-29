using Arise.DDD.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.Domain.AggregatesModel.UserCircleRelationAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.Circle.JoinCircle
{
    public class JoinCircleCommandHandler : IRequestHandler<JoinCircleCommand, bool>
    {
        private readonly IUserCircleRelationRepository _userCircleRelationRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<JoinCircleCommandHandler> _logger;

        public JoinCircleCommandHandler(IUserCircleRelationRepository userCircleRelationRepository,
            IHttpContextAccessor httpContextAccessor,
            ILogger<JoinCircleCommandHandler> logger)
        {
            _userCircleRelationRepository = userCircleRelationRepository ?? throw new ArgumentNullException(nameof(userCircleRelationRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(JoinCircleCommand request, CancellationToken cancellationToken)
        {
            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userCircle = await _userCircleRelationRepository.GetRelationAsync(request.CircleId, myId);

            if (userCircle == null)
            {
                userCircle = new UserCircleRelation(myId, request.CircleId);
                _userCircleRelationRepository.Add(userCircle);
                return await _userCircleRelationRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
            }

            return true;
        }
    }
}
