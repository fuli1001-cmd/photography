using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using Photography.Services.Post.Domain.AggregatesModel.UserPostRelationAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.Post.ToggleLikePost
{
    public class ToggleLikePostCommandHandler : IRequestHandler<ToggleLikePostCommand, bool>
    {
        private readonly IUserPostRelationRepository _userPostRelationRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ToggleLikePostCommandHandler> _logger;

        public ToggleLikePostCommandHandler(IUserPostRelationRepository userPostRelationRepository, IHttpContextAccessor httpContextAccessor, ILogger<ToggleLikePostCommandHandler> logger)
        {
            _userPostRelationRepository = userPostRelationRepository ?? throw new ArgumentNullException(nameof(userPostRelationRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ToggleLikePostCommand request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var userPostRelation = await _userPostRelationRepository.GetAsync(Guid.Parse(userId), request.PostId, UserPostRelationType.Like);
            if (userPostRelation == null)
            {
                userPostRelation = new UserPostRelation(userId, request.PostId);
                userPostRelation.Like();
                _userPostRelationRepository.Add(userPostRelation);
            }
            else
            {
                userPostRelation.UnLike();
                _userPostRelationRepository.Remove(userPostRelation);
            }
            return await _userPostRelationRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
