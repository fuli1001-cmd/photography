using Arise.DDD.Domain.Exceptions;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.API.Query.Interfaces;
using Photography.Services.Post.API.Query.ViewModels;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using Photography.Services.Post.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.Post.UpdatePost
{
    public class UpdatePostCommandHandler : IRequestHandler<UpdatePostCommand, PostViewModel>
    {
        private readonly IPostRepository _postRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPostQueries _postQueries;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UpdatePostCommandHandler> _logger;

        public UpdatePostCommandHandler(IPostRepository postRepository, IUserRepository userRepository, IHttpContextAccessor httpContextAccessor,
            IPostQueries postQueries, IConfiguration configuration, ILogger<UpdatePostCommandHandler> logger)
        {
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _postQueries = postQueries ?? throw new ArgumentNullException(nameof(postQueries));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PostViewModel> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var user = await _userRepository.GetByIdAsync(userId);

            if (user.IsDisabled())
            {
                var hours = (int)Math.Ceiling((user.DisabledTime.Value - DateTime.UtcNow).TotalHours);
                throw new ClientException($"账号存在违规行为，该功能禁用{hours}小时");
            }

            var post = await _postRepository.GetPostWithAttachmentsById(request.PostId);

            if (post == null)
                throw new ClientException("操作失败", new List<string> { $"Post {request.PostId} does not exists." });

            if (post.UserId != userId)
                throw new ClientException("操作失败", new List<string> { $"Post {post.Id} does not belong to user {userId}" });

            var attachments = request.Attachments.Select(a => new PostAttachment(a.Name, a.Text, a.AttachmentType, a.IsPrivate)).ToList();

            post.Update(request.Text, request.Commentable, request.ForwardType, request.ShareType, request.Visibility,
                request.ViewPassword, request.PublicTags, request.PrivateTag, request.CircleId, request.Latitude, request.Longitude, request.LocationName, request.Address,
                request.CityCode, request.FriendIds, attachments, request.ShowOriginalText);

            #region arise内部用户更新帖子：无需审核
            var role = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
            if (role == "internal")
                post.SetPostAuthStatus(PostAuthStatus.Authenticated);
            #endregion

            _postRepository.Update(post);

            if (await _postRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
                return await _postQueries.GetPostAsync(post.Id);

            throw new ApplicationException("操作失败");
        }
    }
}
