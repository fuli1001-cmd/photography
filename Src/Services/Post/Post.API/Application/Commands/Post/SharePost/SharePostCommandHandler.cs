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

namespace Photography.Services.Post.API.Application.Commands.Post.SharePost
{
    public class SharePostCommandHandler : IRequestHandler<SharePostCommand, bool>
    {
        private readonly IUserShareRepository _userShareRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<SharePostCommandHandler> _logger;

        public SharePostCommandHandler(
            IUserShareRepository userShareRepository,
            IHttpContextAccessor httpContextAccessor,
            ILogger<SharePostCommandHandler> logger)
        {
            _userShareRepository = userShareRepository ?? throw new ArgumentNullException(nameof(userShareRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(SharePostCommand request, CancellationToken cancellationToken)
        {
            var shareUser = true;
            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (request.PostIds != null)
            {
                shareUser = request.PostIds.Count == 0;

                request.PostIds.ForEach(postId =>
                {
                    var userShare = new UserShare();
                    userShare.SharePost(myId, postId);
                });
            }

            if (request.PrivateTagIds != null)
            {
                shareUser = request.PrivateTagIds.Count == 0;

                request.PrivateTagIds.ForEach(privateTagId =>
                {
                    var userShare = new UserShare();
                    userShare.ShareTag(myId, privateTagId);
                });
            }

            if (request.UnSpecifiedPrivateTag)
            {
                shareUser = false;

                var userShare = new UserShare();
                userShare.ShareUnSpecifiedTag(myId);
            }

            if (shareUser)
            {
                var userShare = new UserShare();
                userShare.ShareUser(myId);
            }

            return await _userShareRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
