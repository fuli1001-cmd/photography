﻿using Arise.DDD.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.API.Query.Interfaces;
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
    public class SharePostCommandHandler : IRequestHandler<SharePostCommand, bool>
    {
        private readonly IPostRepository _postRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<SharePostCommandHandler> _logger;

        public SharePostCommandHandler(
            IPostRepository postRepository,
            IHttpContextAccessor httpContextAccessor,
            ILogger<SharePostCommandHandler> logger)
        {
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(SharePostCommand request, CancellationToken cancellationToken)
        {
            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var posts = await _postRepository.GetPostsAsync(request.PostIds);

            posts.ForEach(p => p.Share(myId));

            return await _postRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
