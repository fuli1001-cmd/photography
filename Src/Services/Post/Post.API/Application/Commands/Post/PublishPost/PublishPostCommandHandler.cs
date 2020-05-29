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

namespace Photography.Services.Post.API.Application.Commands.Post.PublishPost
{
    public class PublishPostCommandHandler : IRequestHandler<PublishPostCommand, PostViewModel>
    {
        private readonly IPostRepository _postRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly ILogger<PublishPostCommandHandler> _logger;
        private readonly IServiceProvider _serviceProvider;

        private IMessageSession _messageSession;

        public PublishPostCommandHandler(IPostRepository postRepository, IHttpContextAccessor httpContextAccessor,
            IServiceProvider serviceProvider, IMapper mapper, ILogger<PublishPostCommandHandler> logger)
        {
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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
                await SendPostPublishedEventAsync(userId);
            _postRepository.LoadUser(post);
            return _mapper.Map<PostViewModel>(post);
        }

        private async Task SendPostPublishedEventAsync(Guid userId)
        {
            var @event = new PostPublishedEvent { UserId = userId };
            _messageSession = (IMessageSession)_serviceProvider.GetService(typeof(IMessageSession));
            await _messageSession.Publish(@event);
            _logger.LogInformation("----- Published PostPublishedEvent: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);
        }
    }
}
