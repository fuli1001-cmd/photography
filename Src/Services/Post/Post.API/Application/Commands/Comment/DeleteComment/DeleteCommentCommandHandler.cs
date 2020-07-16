using Arise.DDD.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.Domain.AggregatesModel.CommentAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.Comment.DeleteComment
{
    public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, bool>
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<DeleteCommentCommandHandler> _logger;

        public DeleteCommentCommandHandler(
            ICommentRepository commentRepository,
            IHttpContextAccessor httpContextAccessor,
            ILogger<DeleteCommentCommandHandler> logger)
        {
            _commentRepository = commentRepository ?? throw new ArgumentNullException(nameof(commentRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
        {
            var comment = await _commentRepository.GetCommentAsync(request.CommentId);

            if (comment == null)
                throw new ClientException("操作失败", new List<string> { $"Comment {request.CommentId} does not exist." });

            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var role = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;

            if (comment.Id != myId && role != "admin")
                throw new ClientException("操作失败", new List<string> { $"Comment {request.CommentId} does not belong to user {myId}" });
            
            comment.Delete();

            _commentRepository.Update(comment);
            _commentRepository.Remove(comment);

            return await _commentRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
