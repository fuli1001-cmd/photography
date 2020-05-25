using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.Domain.AggregatesModel.CommentAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.Comment.LikeComment
{
    public class LikeCommentCommandHandler : IRequestHandler<LikeCommentCommand, bool>
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<LikeCommentCommandHandler> _logger;

        public LikeCommentCommandHandler(ICommentRepository commentRepository, IHttpContextAccessor httpContextAccessor, ILogger<LikeCommentCommandHandler> logger)
        {
            _commentRepository = commentRepository ?? throw new ArgumentNullException(nameof(commentRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<bool> Handle(LikeCommentCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
