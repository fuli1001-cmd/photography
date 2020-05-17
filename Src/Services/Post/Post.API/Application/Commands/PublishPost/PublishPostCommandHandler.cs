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

namespace Photography.Services.Post.API.Application.Commands.PublishPost
{
    public class PublishPostCommandHandler : IRequestHandler<PublishPostCommand, SameCityPostViewModel>
    {
        private readonly IPostRepository _postRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly ILogger<PublishPostCommandHandler> _logger;

        public PublishPostCommandHandler(IPostRepository postRepository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper, ILogger<PublishPostCommandHandler> logger)
        {
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<SameCityPostViewModel> Handle(PublishPostCommand request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var attachments = request.attachments.Select(a => new PostAttachment(a.Name, a.Text, a.ContentType)).ToList();
            var post = new Domain.AggregatesModel.PostAggregate.Post(request.Text, request.Commentable, request.ForwardType, request.ShareType,
                request.Visibility, request.ViewPassword, request.Province, request.City, request.Latitude, request.Longitude, request.LocationName,
                request.Address, request.friendIds, attachments, Guid.Parse(userId));
            _postRepository.Add(post);
            await _postRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
            _postRepository.LoadUser(post);
            return _mapper.Map<SameCityPostViewModel>(post);
        }
    }
}
