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

namespace Photography.Services.Post.API.Application.Commands.Comment.ReplyPost
{
    public class ReplyPostCommandHandler : IRequestHandler<ReplyPostCommand, int>
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IPostRepository _postRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ReplyPostCommandHandler> _logger;

        public ReplyPostCommandHandler(ICommentRepository commentRepository,
            IPostRepository postRepository,
            IHttpContextAccessor httpContextAccessor, 
            ILogger<ReplyPostCommandHandler> logger)
        {
            _commentRepository = commentRepository ?? throw new ArgumentNullException(nameof(commentRepository));
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<int> Handle(ReplyPostCommand request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var comment = new Domain.AggregatesModel.CommentAggregate.Comment();
            comment.ReplyPost(request.Text, request.PostId, userId);
            _commentRepository.Add(comment);
            await _commentRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
            //返回帖子评论的总数量
            return await _postRepository.GetPostCommentCountAsync(request.PostId);
        }
    }
}
