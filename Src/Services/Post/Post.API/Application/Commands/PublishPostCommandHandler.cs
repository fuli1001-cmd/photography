using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands
{
    public class PublishPostCommandHandler : IRequestHandler<PublishPostCommand, bool>
    {
        //private readonly IPostRepository _postRepository;
        private readonly ILogger<PublishPostCommandHandler> _logger;

        public PublishPostCommandHandler(ILogger<PublishPostCommandHandler> logger)
        {
            //_gamePlayerRepository = gamePlayerRepository ?? throw new ArgumentNullException(nameof(gamePlayerRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<bool> Handle(PublishPostCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
