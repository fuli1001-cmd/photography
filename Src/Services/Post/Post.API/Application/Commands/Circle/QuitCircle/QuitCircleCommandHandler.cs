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

namespace Photography.Services.Post.API.Application.Commands.Circle.QuitCircle
{
    public class QuitCircleCommandHandler : IRequestHandler<QuitCircleCommand, bool>
    {
        private readonly ICircleRepository _circleRepository;
        private readonly IUserCircleRelationRepository _userCircleRelationRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<QuitCircleCommandHandler> _logger;

        public QuitCircleCommandHandler(ICircleRepository circleRepository,
            IUserCircleRelationRepository userCircleRelationRepository,
            IHttpContextAccessor httpContextAccessor,
            ILogger<QuitCircleCommandHandler> logger)
        {
            _circleRepository = circleRepository ?? throw new ArgumentNullException(nameof(circleRepository));
            _userCircleRelationRepository = userCircleRelationRepository ?? throw new ArgumentNullException(nameof(userCircleRelationRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(QuitCircleCommand request, CancellationToken cancellationToken)
        {
            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var userCircle = await _userCircleRelationRepository.GetRelationAsync(request.CircleId, myId);

            if (userCircle == null)
                return true;

            var circle = await _circleRepository.GetByIdAsync(request.CircleId);
            if (circle.OwnerId == myId)
                throw new ClientException("操作失败", new List<string> { $"User {myId} is the owner of the circle {request.CircleId}, can't quit circle." });

            userCircle.Quit();
            _userCircleRelationRepository.Remove(userCircle);

            return await _userCircleRelationRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
