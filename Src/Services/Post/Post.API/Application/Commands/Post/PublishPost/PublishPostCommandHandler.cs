using ApplicationMessages.Events;
using Arise.DDD.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NServiceBus;
using Photography.Services.Post.API.Query.Interfaces;
using Photography.Services.Post.API.Query.ViewModels;
using Photography.Services.Post.API.Settings;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using Photography.Services.Post.Domain.AggregatesModel.TagAggregate;
using Photography.Services.Post.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.Post.PublishPost
{
    public class PublishPostCommandHandler : IRequestHandler<PublishPostCommand, PostViewModel>
    {
        private readonly IPostRepository _postRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IPostQueries _postQueries;
        private readonly PostScoreRewardSettings _scoreRewardSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<PublishPostCommandHandler> _logger;
        private readonly IServiceProvider _serviceProvider;

        private IMessageSession _messageSession;

        public PublishPostCommandHandler(IPostRepository postRepository, 
            IUserRepository userRepository, 
            ITagRepository tagRepository,
            IPostQueries postQueries,
            IOptionsSnapshot<PostScoreRewardSettings> scoreRewardOptions,
            IHttpContextAccessor httpContextAccessor,
            IServiceProvider serviceProvider, 
            ILogger<PublishPostCommandHandler> logger)
        {
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _tagRepository = tagRepository ?? throw new ArgumentNullException(nameof(tagRepository));
            _postQueries = postQueries ?? throw new ArgumentNullException(nameof(postQueries));
            _scoreRewardSettings = scoreRewardOptions?.Value ?? throw new ArgumentNullException(nameof(scoreRewardOptions));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PostViewModel> Handle(PublishPostCommand request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = await _userRepository.GetByIdAsync(userId);

            if (user.IsDisabled())
            {
                var hours = (int)Math.Ceiling((user.DisabledTime.Value - DateTime.UtcNow).TotalHours);
                throw new ClientException($"账号存在违规行为，该功能禁用{hours}小时");
            }

            // check if the private tag name exist
            if (!string.IsNullOrWhiteSpace(request.PrivateTag))
            {
                var tag = await _tagRepository.GetUserPrivateTagByName(userId, request.PrivateTag);
                if (tag == null)
                    throw new ClientException("操作失败", new List<string> { $"Tag {request.PrivateTag} does not exist."});
            }

            var score = (DateTime.Now - DateTime.UnixEpoch.AddSeconds(user.CreatedTime)).TotalHours <= _scoreRewardSettings.NewUserHour ? user.Score + _scoreRewardSettings.NewUserPost : user.Score;
            var attachments = request.Attachments.Select(a => new PostAttachment(a.Name, a.Text, a.AttachmentType, a.IsPrivate)).ToList();
            var post = Domain.AggregatesModel.PostAggregate.Post.CreatePost(request.Text, request.Commentable, request.ForwardType, request.ShareType,
                request.Visibility, request.ViewPassword, request.PublicTags, request.PrivateTag, request.CircleId, request.Latitude, request.Longitude, request.LocationName,
                request.Address, request.CityCode, request.FriendIds, attachments, score, userId);

            _postRepository.Add(post);

            if (await _postRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
            {
                // 取得附件中的第一张图片，然后发出“帖子已发布事件”
                string image = null;
                var first = request.Attachments[0];
                if (first.AttachmentType == AttachmentType.Image)
                    image = first.Name;
                else if (first.AttachmentType == AttachmentType.Video)
                    image = first.Name.Substring(0, first.Name.LastIndexOf('.')) + ".jpg";

                await SendPostPublishedEventAsync(userId, post.Id, image);

                return await _postQueries.GetPostAsync(post.Id);
            }

            throw new ApplicationException("操作失败");
        }

        private async Task SendPostPublishedEventAsync(Guid userId, Guid postId, string image)
        {
            var @event = new PostPublishedEvent { UserId = userId, PostId = postId, Image = image };
            _messageSession = (IMessageSession)_serviceProvider.GetService(typeof(IMessageSession));
            await _messageSession.Publish(@event);
            _logger.LogInformation("----- Published PostPublishedEvent: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);
        }
    }
}
