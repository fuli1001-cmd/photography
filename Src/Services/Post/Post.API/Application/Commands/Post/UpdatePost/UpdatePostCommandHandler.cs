using Arise.DDD.Domain.Exceptions;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.API.Query.Interfaces;
using Photography.Services.Post.API.Query.ViewModels;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.Post.UpdatePost
{
    public class UpdatePostCommandHandler : IRequestHandler<UpdatePostCommand, PostViewModel>
    {
        private readonly IPostRepository _postRepository;
        private readonly IPostQueries _postQueries;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UpdatePostCommandHandler> _logger;

        public UpdatePostCommandHandler(IPostRepository postRepository, IHttpContextAccessor httpContextAccessor,
            IPostQueries postQueries, ILogger<UpdatePostCommandHandler> logger)
        {
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _postQueries = postQueries ?? throw new ArgumentNullException(nameof(postQueries));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PostViewModel> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
        {
            var post = await _postRepository.GetPostWithAttachmentsById(request.PostId);

            if (post == null)
                throw new ClientException("操作失败。", new List<string> { $"Post {request.PostId} does not exists." });

            var userId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if (post.UserId != userId)
                throw new ClientException("操作失败。", new List<string> { $"Post {post.Id} does not belong to user {userId}" });

            var attachments = request.Attachments.Select(a => new PostAttachment(a.Name, a.Text, a.AttachmentType)).ToList();

            post.Update(request.Text, request.Commentable, request.ForwardType, request.ShareType, request.Visibility,
                request.ViewPassword, request.Latitude, request.Longitude, request.LocationName, request.Address,
                request.CityCode, request.FriendIds, attachments);

            _postRepository.Update(post);

            if (await _postRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
                return await _postQueries.GetPostAsync(post.Id);

            throw new ApplicationException("操作失败。");
        }
    }
}
