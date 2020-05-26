using Arise.DDD.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.Domain.AggregatesModel.CommentAggregate;
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
        private readonly ICommentRepository _commentRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ToggleLikeCommentCommandHandler> _logger;

        public ToggleLikeCommentCommandHandler(IUserCommentRelationRepository userCommentRelationRepository, ICommentRepository commentRepository,
            IHttpContextAccessor httpContextAccessor, ILogger<ToggleLikeCommentCommandHandler> logger)
        {
            _userCommentRelationRepository = userCommentRelationRepository ?? throw new ArgumentNullException(nameof(userCommentRelationRepository));
            _commentRepository = commentRepository ?? throw new ArgumentNullException(nameof(commentRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ToggleLikeCommentCommand request, CancellationToken cancellationToken)
        {
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
            }
            else
            {
                userCommentRelation.UnLike(comment.PostId);
                _userCommentRelationRepository.Remove(userCommentRelation);
            }

            return await _userCommentRelationRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
