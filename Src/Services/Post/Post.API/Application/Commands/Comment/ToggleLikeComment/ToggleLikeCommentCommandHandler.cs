using ApplicationMessages.Events;
using Arise.DDD.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.Post.Domain.AggregatesModel.CommentAggregate;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using Photography.Services.Post.Domain.AggregatesModel.UserCommentRelationAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.Comment.ToggleLikeComment
{
    public class ToggleLikeCommentCommandHandler : IRequestHandler<ToggleLikeCommentCommand, bool>
    {
        private readonly IUserCommentRelationRepository _userCommentRelationRepository;
        private readonly IPostRepository _postRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ToggleLikeCommentCommandHandler> _logger;
        private readonly IServiceProvider _serviceProvider;

        private IMessageSession _messageSession;

        public ToggleLikeCommentCommandHandler(
            IUserCommentRelationRepository userCommentRelationRepository,
            IPostRepository postRepository,
            ICommentRepository commentRepository,
            IServiceProvider serviceProvider,
            IHttpContextAccessor httpContextAccessor, 
            ILogger<ToggleLikeCommentCommandHandler> logger)
        {
            _userCommentRelationRepository = userCommentRelationRepository ?? throw new ArgumentNullException(nameof(userCommentRelationRepository));
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _commentRepository = commentRepository ?? throw new ArgumentNullException(nameof(commentRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ToggleLikeCommentCommand request, CancellationToken cancellationToken)
        {
            bool result = false;

            var userId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var comment = await _commentRepository.GetByIdAsync(request.CommentId);
            if (comment == null)
                throw new DomainException("评论不存在。");

            var userCommentRelation = await _userCommentRelationRepository.GetAsync(userId, request.CommentId);

            if (userCommentRelation == null)
            {
                userCommentRelation = new UserCommentRelation(userId, request.CommentId);
                userCommentRelation.Like(comment.PostId);
                _userCommentRelationRepository.Add(userCommentRelation);

                result = await _userCommentRelationRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
                if (result)
                    await SendCommentLikedEventAsync(userId, comment);
            }
            else
            {
                userCommentRelation.UnLike(comment.PostId);
                _userCommentRelationRepository.Remove(userCommentRelation);

                result = await _userCommentRelationRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
                if (result)
                    await SendPostUnLikedEventAsync(comment.PostId);
            }

            return result;
        }

        private async Task SendCommentLikedEventAsync(Guid likingUserId, Domain.AggregatesModel.CommentAggregate.Comment comment)
        {
            var post = await _postRepository.GetByIdAsync(comment.PostId);
            var @event = new CommentLikedEvent 
            {
                LikingUserId = likingUserId,
                PostUserId = post.UserId,
                PostId = comment.PostId,
                CommentUserId = comment.UserId,
                CommentId = comment.Id,
                CommentText = comment.Text
            };
            await SendEvent(@event);
        }

        private async Task SendPostUnLikedEventAsync(Guid postId)
        {
            var post = await _postRepository.GetByIdAsync(postId);
            var @event = new PostUnLikedEvent { PostUserId = post.UserId };
            await SendEvent(@event);
        }

        private async Task SendEvent(BaseEvent @event)
        {
            _messageSession = (IMessageSession)_serviceProvider.GetService(typeof(IMessageSession));
            await _messageSession.Publish(@event);
            _logger.LogInformation("----- Published {IntegrationEventName}: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", @event.GetType().Name, @event.Id, Program.AppName, @event);
        }
    }
}
