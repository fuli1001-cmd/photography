using MediatR;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands
{
    public class PublishPostCommandHandler : IRequestHandler<PublishPostCommand, bool>
    {
        private readonly IPostRepository _postRepository;
        private readonly ILogger<PublishPostCommandHandler> _logger;

        public PublishPostCommandHandler(IPostRepository postRepository, ILogger<PublishPostCommandHandler> logger)
        {
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(PublishPostCommand request, CancellationToken cancellationToken)
        {
            var attachments = request.attachments.Select(a => new PostAttachment(a.Name, a.Text)).ToList();
            var post = new Domain.AggregatesModel.PostAggregate.Post(request.Text, request.Commentable, request.ForwardType, request.ShareType,
                request.Visibility, request.ViewPassword, request.Province, request.City, request.Latitude, request.Longitude, request.LocationName,
                request.Address, request.friendIds, attachments);
            await _postRepository.AddAsync(post);
            return await _postRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
