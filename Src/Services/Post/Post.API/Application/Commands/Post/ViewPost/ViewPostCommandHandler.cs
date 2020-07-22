using Arise.DDD.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Photography.Services.Post.API.Settings;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.Post.ViewPost
{
    public class ViewPostCommandHandler : IRequestHandler<ViewPostCommand, bool>
    {
        private readonly IPostRepository _postRepository;
        private readonly PostScoreRewardSettings _scoreRewardSettings;
        private readonly ILogger<ViewPostCommandHandler> _logger;

        public ViewPostCommandHandler(
            IPostRepository postRepository,
            IOptionsSnapshot<PostScoreRewardSettings> scoreRewardOptions,
            ILogger<ViewPostCommandHandler> logger)
        {
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _scoreRewardSettings = scoreRewardOptions?.Value ?? throw new ArgumentNullException(nameof(scoreRewardOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ViewPostCommand request, CancellationToken cancellationToken)
        {
            var post = await _postRepository.GetByIdAsync(request.PostId);
            post.View(_scoreRewardSettings.ViewPost);

            if (await _postRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
                return true;

            throw new ApplicationException("操作失败");
        }
    }
}
