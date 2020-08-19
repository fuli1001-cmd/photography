using ApplicationMessages.Events;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NServiceBus;
using Photography.Services.Post.API.Query.Interfaces;
using Photography.Services.Post.API.Query.ViewModels;
using Photography.Services.Post.API.Settings;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using Photography.Services.Post.Domain.AggregatesModel.UserAggregate;
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
        private readonly IUserRepository _userRepository;
        private readonly IPostQueries _postQueries;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly PostScoreRewardSettings _scoreRewardSettings;
        private readonly ILogger<ForwardPostsCommandHandler> _logger;
        private readonly IServiceProvider _serviceProvider;

        private IMessageSession _messageSession;

        public ForwardPostsCommandHandler(
            IPostRepository postRepository, 
            IUserRepository userRepository, 
            IPostQueries postQueries,
            IHttpContextAccessor httpContextAccessor,
            IOptionsSnapshot<PostScoreRewardSettings> scoreRewardOptions,
            IServiceProvider serviceProvider, 
            ILogger<ForwardPostsCommandHandler> logger)
        {
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _postQueries = postQueries ?? throw new ArgumentNullException(nameof(postQueries));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _scoreRewardSettings = scoreRewardOptions?.Value ?? throw new ArgumentNullException(nameof(scoreRewardOptions));
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
            var me = await _userRepository.GetByIdAsync(myId);
            var posts = new List<Domain.AggregatesModel.PostAggregate.Post>();

            toBeForwardedPosts.ForEach(toBeForwardedPost =>
            {
                // 创建新帖子
                var score = (DateTime.Now - DateTime.UnixEpoch.AddSeconds(me.CreatedTime)).TotalHours <= _scoreRewardSettings.NewUserHour ? me.PostScore + _scoreRewardSettings.NewUserPost : me.PostScore;
                var post = Domain.AggregatesModel.PostAggregate.Post.CreatePost(request.Text, request.Commentable, request.ForwardType, request.ShareType,
                    request.Visibility, request.ViewPassword, request.SystemTag, request.PublicTags, request.PrivateTag, null, request.Latitude, request.Longitude, request.LocationName,
                    request.Address, request.CityCode, request.FriendIds, null, score, myId, request.ShowOriginalText);

                post.SetForwardPostId(toBeForwardedPost);
                _postRepository.Add(post);
                posts.Add(post);

                // 增加被转发帖子的转发数和积分
                toBeForwardedPost.IncreaseForwardCount(_scoreRewardSettings.ForwardPost);

                // 被转发帖子是转发贴时，还要增加原始帖的转发数和积分
                if (toBeForwardedPost.ForwardedPostId != null)
                {
                    var originalPost = originalPosts.FirstOrDefault(p => p.Id == toBeForwardedPost.ForwardedPostId.Value);
                    originalPost.IncreaseForwardCount(_scoreRewardSettings.ForwardPost);
                }
            });

            if (await _postRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
            {
                var forwardInfos = new List<ForwardInfo>();

                foreach (var post in posts)
                {
                    var forwardedPost = toBeForwardedPosts.FirstOrDefault(p => p.Id == post.ForwardedPostId.Value);

                    if (forwardedPost == null)
                        forwardedPost = originalPosts.FirstOrDefault(p => p.Id == post.ForwardedPostId.Value);

                    // 如果帖子为审核通过，取得新帖和被转发帖中被@的用户id以便发送被@通知
                    IEnumerable<Guid> atUserIds = new List<Guid>();
                    if (post.PostAuthStatus == PostAuthStatus.Authenticated)
                        atUserIds = await _postQueries.GetAtUserIdsAsync(post);

                    forwardInfos.Add(new ForwardInfo
                    {
                        ForwardUserId = myId,
                        OriginalPostUserId = forwardedPost.UserId,
                        OriginalPostId = post.ForwardedPostId.Value,
                        NewPostId = post.Id,
                        AtUserIds = atUserIds
                    });
                }

                await SendPostForwardedEventAsync(forwardInfos);
            }

            return await _postQueries.GetPostsAsync(posts.Select(p => p.Id).ToList());
        }

        private async Task SendPostForwardedEventAsync(List<ForwardInfo> forwardInfos)
        {
            var @event = new PostForwardedEvent { ForwardInfos = forwardInfos };
            _messageSession = (IMessageSession)_serviceProvider.GetService(typeof(IMessageSession));
            await _messageSession.Publish(@event);
            _logger.LogInformation("----- Published PostForwardedEvent: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);
        }
    }
}
