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

namespace Photography.Services.Post.API.Application.Commands.Circle.ToppingCircle
{
    public class ToppingCircleCommandHandler : IRequestHandler<ToppingCircleCommand, bool>
    {
        private readonly IUserCircleRelationRepository _userCircleRelationRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ToppingCircleCommandHandler> _logger;

        public ToppingCircleCommandHandler(IUserCircleRelationRepository userCircleRelationRepository,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ToppingCircleCommandHandler> logger)
        {
            _userCircleRelationRepository = userCircleRelationRepository ?? throw new ArgumentNullException(nameof(userCircleRelationRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ToppingCircleCommand request, CancellationToken cancellationToken)
        {
            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            // 需要置顶或取消置顶的圈子
            var relation = await _userCircleRelationRepository.GetRelationAsync(request.CircleId, myId);
            if (relation == null)
                throw new ClientException("操作失败", new List<string> { $"User {myId} is not in circle {request.CircleId}" });

            if (request.Topping)
            {
                // 原来置顶的圈子
                var oldToppingRelation = await _userCircleRelationRepository.GetToppingCircleRelationAsync(myId);
                if (oldToppingRelation.CircleId == request.CircleId)
                    return true;

                relation.ToppingCircle();
                oldToppingRelation.UnToppingCircle();
            }
            else
            {
                relation.UnToppingCircle();
            }

            return await _userCircleRelationRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
