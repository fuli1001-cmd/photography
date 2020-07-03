using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using Photography.Services.Post.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.DomainEventHandlers.UserShared
{
    public class UserSharedDomainEventHandler : INotificationHandler<UserSharedDomainEvent>
    {
        private readonly IPostRepository _postRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UserSharedDomainEventHandler> _logger;

        public UserSharedDomainEventHandler(
            IPostRepository postRepository,
            IHttpContextAccessor httpContextAccessor,
            ILogger<UserSharedDomainEventHandler> logger)
        {
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(UserSharedDomainEvent notification, CancellationToken cancellationToken)
        {
            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var posts = await _postRepository.GetUserPostsAsync(myId);
            posts.ForEach(p => p.Share());
        }
    }
}
