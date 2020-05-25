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

namespace Photography.Services.Post.API.Application.Commands.Comment.ReplyComment
{
    public class ReplyCommentCommandHandler : IRequestHandler<ReplyCommentCommand, bool>
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ReplyCommentCommandHandler> _logger;

        public ReplyCommentCommandHandler(ICommentRepository commentRepository, IHttpContextAccessor httpContextAccessor, ILogger<ReplyCommentCommandHandler> logger)
        {
            _commentRepository = commentRepository ?? throw new ArgumentNullException(nameof(commentRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ReplyCommentCommand request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var comment = new Domain.AggregatesModel.CommentAggregate.Comment(request.Text, null, request.CommentId, userId);
            _commentRepository.Add(comment);
            return await _commentRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
