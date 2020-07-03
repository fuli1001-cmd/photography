using Arise.DDD.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.API.Query.Interfaces;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using Photography.Services.Post.Domain.AggregatesModel.UserPostRelationAggregate;
using Photography.Services.Post.Domain.AggregatesModel.UserShareAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.Post.Share
{
    public class ShareCommandHandler : IRequestHandler<ShareCommand, bool>
    {
        private readonly IUserShareRepository _userShareRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ShareCommandHandler> _logger;

        public ShareCommandHandler(
            IUserShareRepository userShareRepository,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ShareCommandHandler> logger)
        {
            _userShareRepository = userShareRepository ?? throw new ArgumentNullException(nameof(userShareRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ShareCommand request, CancellationToken cancellationToken)
        {
            var shared = false;
            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (request.PostIds != null)
            {
                request.PostIds.ForEach(postId =>
                {
                    var userShare = new UserShare();
                    userShare.SharePost(myId, postId,request.NoAd);
                    shared = true;
                });
            }

            if (request.PrivateTagNames != null)
            {
                request.PrivateTagNames.ForEach(privateTagName =>
                {
                    var userShare = new UserShare();
                    userShare.ShareTag(myId, privateTagName, request.NoAd);
                    shared = true;
                });
            }

            // 既没有分享帖子，分享帖子类别，即分享用户（所有的帖子）
            if (!shared)
            {
                var userShare = new UserShare();
                userShare.ShareUser(myId, request.NoAd);
            }

            return await _userShareRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
