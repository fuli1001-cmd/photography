using Arise.DDD.Domain.Exceptions;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdatePostCommandHandler> _logger;

        public UpdatePostCommandHandler(IPostRepository postRepository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper, ILogger<UpdatePostCommandHandler> logger)
        {
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PostViewModel> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
        {
            var post = await _postRepository.GetPostWithAttachmentsById(request.PostId);
            var userId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if (post.UserId != userId)
                throw new DomainException("更新失败。");

            var attachments = request.Attachments.Select(a => new PostAttachment(a.Name, a.Text, a.AttachmentType)).ToList();

            post.Update(request.Text, request.Commentable, request.ForwardType, request.ShareType, request.Visibility,
                request.ViewPassword, request.Latitude, request.Longitude, request.LocationName, request.Address,
                request.CityCode, request.FriendIds, attachments);

            _postRepository.Update(post);
            if (await _postRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
            {
                _postRepository.LoadUser(post);
                return _mapper.Map<PostViewModel>(post);
            }
            else
                throw new DomainException("更新失败。");
        }
    }
}
