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

namespace Photography.Services.Post.API.Application.Commands.Post.ReplyPost
{
    public class ReplyPostCommandHandler : IRequestHandler<ReplyPostCommand, bool>
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ReplyPostCommandHandler> _logger;

        public ReplyPostCommandHandler(ICommentRepository commentRepository, IHttpContextAccessor httpContextAccessor, ILogger<ReplyPostCommandHandler> logger)
        {
            _commentRepository = commentRepository ?? throw new ArgumentNullException(nameof(commentRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ReplyPostCommand request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var comment = new Comment(request.Text, request.PostId, null, userId);
            _commentRepository.Add(comment);
            return await _commentRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
            //_postRepository.LoadUser(post);
            //return _mapper.Map<PostViewModel>(post);
        }
    }
}
