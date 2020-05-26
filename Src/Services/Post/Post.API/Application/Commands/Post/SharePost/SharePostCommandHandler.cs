using Arise.DDD.Domain.Exceptions;
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

namespace Photography.Services.Post.API.Application.Commands.Post.SharePost
{
    public class SharePostCommandHandler : IRequestHandler<SharePostCommand, int>
    {
        private readonly IPostRepository _postRepository;
        private readonly ILogger<SharePostCommandHandler> _logger;

        public SharePostCommandHandler(IPostRepository postRepository, ILogger<SharePostCommandHandler> logger)
        {
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<int> Handle(SharePostCommand request, CancellationToken cancellationToken)
        {
            var post = await _postRepository.GetByIdAsync(request.PostId);
            if (post == null)
                throw new DomainException("帖子不存在");
            post.Share();
            await _postRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
            return post.ShareCount;
        }
    }
}
