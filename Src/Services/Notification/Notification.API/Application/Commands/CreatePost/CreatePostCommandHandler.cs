using MediatR;
using Microsoft.Extensions.Logging;
using Photography.Services.Notification.Domain.AggregatesModel.PostAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Notification.API.Application.Commands.CreatePost
{
    public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, bool>
    {
        private readonly IPostRepository _postRepository;
        private readonly ILogger<CreatePostCommandHandler> _logger;

        public CreatePostCommandHandler(IPostRepository postRepository, ILogger<CreatePostCommandHandler> logger)
        {
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(CreatePostCommand request, CancellationToken cancellationToken)
        {
            var post = new Post(request.PostId, request.Image);
            _postRepository.Add(post);
            return await _postRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
