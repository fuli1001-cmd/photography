using Arise.DDD.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.Domain.AggregatesModel.CircleAggregate;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.Post.MovePostOutFromCircle
{
    public class MovePostOutFromCircleCommandHandler : IRequestHandler<MovePostOutFromCircleCommand, bool>
    {
        private readonly IPostRepository _postRepository;
        private readonly ICircleRepository _circleRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<MovePostOutFromCircleCommandHandler> _logger;

        public MovePostOutFromCircleCommandHandler(IPostRepository postRepository,
            ICircleRepository circleRepository,
            IHttpContextAccessor httpContextAccessor,
            ILogger<MovePostOutFromCircleCommandHandler> logger)
        {
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _circleRepository = circleRepository ?? throw new ArgumentNullException(nameof(circleRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(MovePostOutFromCircleCommand request, CancellationToken cancellationToken)
        {
            var post = await _postRepository.GetByIdAsync(request.PostId);
            
            if (post == null)
                throw new ClientException("操作失败", new List<string> { $"Post {request.PostId} does not exist." });

            if (post.CircleId == null)
                throw new ClientException("操作失败", new List<string> { $"Post {request.PostId} is not in any circle." });

            var circle = await _circleRepository.GetByIdAsync(post.CircleId.Value);

            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (circle.OwnerId != myId)
                throw new ClientException("操作失败", new List<string> { $"User {myId} is not the owner of circle {circle.Id}" });

            post.MoveOutFromCircle();

            return await _postRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
