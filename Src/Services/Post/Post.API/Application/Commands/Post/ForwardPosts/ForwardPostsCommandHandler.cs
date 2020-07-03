using ApplicationMessages.Events;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.Post.API.Query.Interfaces;
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
        private readonly IPostQueries _postQueries;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ForwardPostsCommandHandler> _logger;
        private readonly IServiceProvider _serviceProvider;

        private IMessageSession _messageSession;

        public ForwardPostsCommandHandler(IPostRepository postRepository, IHttpContextAccessor httpContextAccessor,
            IPostQueries postQueries, IServiceProvider serviceProvider, ILogger<ForwardPostsCommandHandler> logger)
        {
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _postQueries = postQueries ?? throw new ArgumentNullException(nameof(postQueries));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<PostViewModel>> Handle(ForwardPostsCommand request, CancellationToken cancellationToken)
        {
            // 被转发的帖子对象list
            var toBeForwardedPosts = await _postRepository.GetPostsAsync(request.ForwardPostIds);

            // 被转发的帖子转发过的原始帖子对象list
            var originalPosts = await _postRepository.GetPostsAsync(toBeForwardedPosts.Where(p => p.ForwardedPostId != null).Select(p => p.ForwardedPostId.Value).ToList());

            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var posts = new List<Domain.AggregatesModel.PostAggregate.Post>();

            toBeForwardedPosts.ForEach(toBeForwardedPost =>
            {
                // 创建新帖子
                var post = Domain.AggregatesModel.PostAggregate.Post.CreatePost(request.Text, request.Commentable, request.ForwardType, request.ShareType,
                    request.Visibility, request.ViewPassword, request.PublicTags, request.PrivateTag, null, request.Latitude, request.Longitude, request.LocationName,
                    request.Address, request.CityCode, request.FriendIds, null, myId, request.ShowOriginalText);
                post.SetForwardPostId(toBeForwardedPost.ForwardedPostId ?? toBeForwardedPost.Id);
                _postRepository.Add(post);
                posts.Add(post);

                // 增加被转发帖子的转发数
                toBeForwardedPost.IncreaseForwardCount();

                // 被转发帖子是转发贴时，还要增加原始帖的转发数
                if (toBeForwardedPost.ForwardedPostId != null)
                {
                    var originalPost = originalPosts.FirstOrDefault(p => p.Id == toBeForwardedPost.ForwardedPostId.Value);
                    originalPost.IncreaseForwardCount();
                }
            });

            if (await _postRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
            {
                //var postUserIds = await _postRepository.GetPostsUserIdsAsync(request.ForwardPostIds);
                foreach(var p in posts)
                {
                    _logger.LogInformation("*************************** ForwardedPostId: {ForwardedPostId}", p.ForwardedPostId ?? Guid.Empty);
                    var forwardedPost = toBeForwardedPosts.FirstOrDefault(p => p.Id == p.ForwardedPostId);
                    if (forwardedPost == null)
                        forwardedPost = originalPosts.FirstOrDefault(p => p.Id == p.ForwardedPostId);

                    await SendPostForwardedEventAsync(myId, forwardedPost.UserId, p.ForwardedPostId.Value, p.Id);
                }
            }

            return await _postQueries.GetPostsAsync(posts.Select(p => p.Id).ToList());
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
