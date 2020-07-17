using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.Post.ExaminePost
{
    public class ExaminePostCommandHandler : IRequestHandler<ExaminePostCommand, bool>
    {
        private readonly IPostRepository _postRepository;
        private readonly ILogger<ExaminePostCommandHandler> _logger;
        private readonly IServiceProvider _serviceProvider;

        private IMessageSession _messageSession;

        public ExaminePostCommandHandler(IPostRepository postRepository,
            IServiceProvider serviceProvider,
            ILogger<ExaminePostCommandHandler> logger)
        {
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ExaminePostCommand request, CancellationToken cancellationToken)
        {
            var post = await _postRepository.GetByIdAsync(request.PostId);
            post.Examine(request.PostAuthStatus);
            return await _postRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
