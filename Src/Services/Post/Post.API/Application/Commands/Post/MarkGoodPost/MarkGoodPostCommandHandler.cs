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

namespace Photography.Services.Post.API.Application.Commands.Post.MarkGoodPost
{
    public class MarkGoodPostCommandHandler : IRequestHandler<MarkGoodPostCommand, bool>
    {
        private readonly IPostRepository _postRepository;
        private readonly ICircleRepository _circleRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<MarkGoodPostCommandHandler> _logger;

        public MarkGoodPostCommandHandler(IPostRepository postRepository,
            ICircleRepository circleRepository,
            IHttpContextAccessor httpContextAccessor,
            ILogger<MarkGoodPostCommandHandler> logger)
        {
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _circleRepository = circleRepository ?? throw new ArgumentNullException(nameof(circleRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(MarkGoodPostCommand request, CancellationToken cancellationToken)
        {
            var post = await _postRepository.GetByIdAsync(request.PostId);

            if (post == null)
                throw new ClientException("操作失败", new List<string> { $"Post {request.PostId} does not exist." });

            if (post.CircleId == null)
                throw new ClientException("操作失败", new List<string> { $"Post {request.PostId} is not in any circle." });

            // 圈子主人必须是当前用户
            var circle = await _circleRepository.GetByIdAsync(post.CircleId.Value);
            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (circle.OwnerId != myId)
                throw new ClientException("当前用户不是圈主", new List<string> { $"User {myId} is not the owner of circle {circle.Id}" });

            if (request.Good)
                post.MarkCircleGood();
            else
                post.UnMarkCircleGood();

            return await _postRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
