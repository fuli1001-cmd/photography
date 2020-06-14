using ApplicationMessages.Events;
using Arise.DDD.Domain.Exceptions;
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

namespace Photography.Services.Post.API.Application.Commands.Post.PublishPost
{
    public class PublishPostCommandHandler : IRequestHandler<PublishPostCommand, PostViewModel>
    {
        private readonly IPostRepository _postRepository;
        private readonly IPostQueries _postQueries;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<PublishPostCommandHandler> _logger;
        private readonly IServiceProvider _serviceProvider;

        private IMessageSession _messageSession;

        public PublishPostCommandHandler(IPostRepository postRepository, IHttpContextAccessor httpContextAccessor,
            IPostQueries postQueries, IServiceProvider serviceProvider, ILogger<PublishPostCommandHandler> logger)
        {
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _postQueries = postQueries ?? throw new ArgumentNullException(nameof(postQueries));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PostViewModel> Handle(PublishPostCommand request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var attachments = request.Attachments.Select(a => new PostAttachment(a.Name, a.Text, a.AttachmentType)).ToList();
            var post = Domain.AggregatesModel.PostAggregate.Post.CreatePost(request.Text, request.Commentable, request.ForwardType, request.ShareType,
                request.Visibility, request.ViewPassword, request.Latitude, request.Longitude, request.LocationName,
                request.Address, request.CityCode, request.FriendIds, attachments, userId);
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

            throw new ApplicationException("操作失败。");
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
