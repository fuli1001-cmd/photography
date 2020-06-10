using ApplicationMessages.Events;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.Post.API.Query.ViewModels;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.Post.ForwardPosts
{
    public class ForwardPostsCommandHandler : IRequestHandler<ForwardPostsCommand, IEnumerable<PostViewModel>>
    {
        private readonly IPostRepository _postRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly ILogger<ForwardPostsCommandHandler> _logger;
        private readonly IServiceProvider _serviceProvider;

        private IMessageSession _messageSession;

        public ForwardPostsCommandHandler(IPostRepository postRepository, IHttpContextAccessor httpContextAccessor,
            IServiceProvider serviceProvider, IMapper mapper, ILogger<ForwardPostsCommandHandler> logger)
        {
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<PostViewModel>> Handle(ForwardPostsCommand request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var posts = new List<Domain.AggregatesModel.PostAggregate.Post>();
            
            request.ForwardPostIds.ForEach(forwardPostId =>
            {
                var post = Domain.AggregatesModel.PostAggregate.Post.CreatePost(request.Text, request.Commentable, request.ForwardType, request.ShareType,
                    request.Visibility, request.ViewPassword, request.Latitude, request.Longitude, request.LocationName,
                    request.Address, request.CityCode, request.FriendIds, null, userId, request.ShowOriginalText);
                post.SetForwardPostId(forwardPostId);
                _postRepository.Add(post);
                posts.Add(post);
            });

            if (await _postRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
            {
                var postUserIds = await _postRepository.GetPostsUserIdsAsync(request.ForwardPostIds);
                foreach(var p in posts)
                {
                    await SendPostForwardedEventAsync(userId, postUserIds[p.ForwardedPostId.Value], p.ForwardedPostId.Value, p.Id);
                }
            }

            posts.ForEach(p => _postRepository.LoadUser(p));
            
            return _mapper.Map<List<PostViewModel>>(posts);
        }

        private async Task SendPostForwardedEventAsync(Guid forwardUserId, Guid originalPostUserId, Guid originalPostId, Guid newPostId)
        {
            var @event = new PostForwardedEvent 
            { 
                ForwardUserId = forwardUserId, 
                OriginalPostUserId = originalPostUserId,
                OriginalPostId = originalPostId,
                NewPostId = newPostId
            };
            _messageSession = (IMessageSession)_serviceProvider.GetService(typeof(IMessageSession));
            await _messageSession.Publish(@event);
            _logger.LogInformation("----- Published PostPublishedEvent: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);
        }
    }
}
