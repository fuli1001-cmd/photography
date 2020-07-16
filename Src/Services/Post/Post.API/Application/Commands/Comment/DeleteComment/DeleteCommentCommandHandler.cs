using Arise.DDD.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.Domain.AggregatesModel.CommentAggregate;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.Comment.DeleteComment
{
    public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, int>
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IPostRepository _postRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<DeleteCommentCommandHandler> _logger;

        public DeleteCommentCommandHandler(
            ICommentRepository commentRepository,
            IPostRepository postRepository,
            IHttpContextAccessor httpContextAccessor,
            ILogger<DeleteCommentCommandHandler> logger)
        {
            _commentRepository = commentRepository ?? throw new ArgumentNullException(nameof(commentRepository));
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<int> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
        {
            var comment = await _commentRepository.GetCommentAsync(request.CommentId);

            if (comment == null)
                throw new ClientException("操作失败", new List<string> { $"Comment {request.CommentId} does not exist." });

            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var role = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;

            // 发布该评论的人、发布该评论所属的帖子的人、管理员可以删除该评论
            if (comment.UserId != myId && comment.Post.UserId != myId && role != "admin")
                throw new ClientException("操作失败", new List<string> { $"Comment {request.CommentId} does not belong to user {myId}" });

            // 发送评论被删除的domain event
            comment.Delete();
            // 删除子评论
            comment.SubComments.ToList().ForEach(c => _commentRepository.Remove(c));
            // 删除本评论
            _commentRepository.Remove(comment);

            await _commentRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            return comment.Post.CommentCount;
        }
    }
}
