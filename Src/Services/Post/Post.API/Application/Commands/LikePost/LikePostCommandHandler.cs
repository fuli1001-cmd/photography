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

namespace Photography.Services.Post.API.Application.Commands.LikePost
{
    public class LikePostCommandHandler : IRequestHandler<LikePostCommand, bool>
    {
        private readonly IUserPostRelationRepository _userPostRelationRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<LikePostCommandHandler> _logger;

        public LikePostCommandHandler(IUserPostRelationRepository userPostRelationRepository, IHttpContextAccessor httpContextAccessor, ILogger<LikePostCommandHandler> logger)
        {
            _userPostRelationRepository = userPostRelationRepository ?? throw new ArgumentNullException(nameof(userPostRelationRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(LikePostCommand request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userPostRelation = await _userPostRelationRepository.GetAsync(userId, request.PostId);
            if (userPostRelation == null)
            {
                userPostRelation = new UserPostRelation(userId, request.PostId, UserPostRelationType.Like);
                _userPostRelationRepository.Add(userPostRelation);
                return await _userPostRelationRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
            }
            else
                return true;
        }
    }
}
