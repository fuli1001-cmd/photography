using Arise.DDD.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.Domain.AggregatesModel.CircleAggregate;
using Photography.Services.Post.Domain.AggregatesModel.UserCircleRelationAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.Circle.AddCircleMember
{
    public class AddCircleMemberCommandHandler : IRequestHandler<AddCircleMemberCommand, bool>
    {
        private readonly ICircleRepository _circleRepository;
        private readonly IUserCircleRelationRepository _userCircleRelationRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AddCircleMemberCommandHandler> _logger;
        
        public AddCircleMemberCommandHandler(ICircleRepository circleRepository, 
            IUserCircleRelationRepository userCircleRelationRepository,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AddCircleMemberCommandHandler> logger)
        {
            _circleRepository = circleRepository ?? throw new ArgumentNullException(nameof(circleRepository));
            _userCircleRelationRepository = userCircleRelationRepository ?? throw new ArgumentNullException(nameof(userCircleRelationRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(AddCircleMemberCommand request, CancellationToken cancellationToken)
        {
            var circle = await _circleRepository.GetByIdAsync(request.CircleId);

            if (circle == null)
                throw new ClientException("操作失败", new List<string> { $"Circle {request.CircleId} dos not exist." });

            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (circle.OwnerId != myId)
                throw new ClientException("操作失败", new List<string> { $"Circle {request.CircleId} does not belong to user {myId}" });

            var userCircle = await _userCircleRelationRepository.GetRelationAsync(request.CircleId, request.UserId);

            if (userCircle == null)
            {
                userCircle = new UserCircleRelation(request.UserId, request.CircleId);
                _userCircleRelationRepository.Add(userCircle);
                return await _userCircleRelationRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
            }

            return true;
        }
    }
}
