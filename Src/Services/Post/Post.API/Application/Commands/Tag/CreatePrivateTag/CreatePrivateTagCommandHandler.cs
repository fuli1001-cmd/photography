using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.Domain.AggregatesModel.TagAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.Tag.CreatePrivateTag
{
    public class CreatePrivateTagCommandHandler : IRequestHandler<CreatePrivateTagCommand, bool>
    {
        private readonly ITagRepository _tagRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CreatePrivateTagCommandHandler> _logger;

        public CreatePrivateTagCommandHandler(ITagRepository tagRepository,
            IHttpContextAccessor httpContextAccessor,
            ILogger<CreatePrivateTagCommandHandler> logger)
        {
            _tagRepository = tagRepository ?? throw new ArgumentNullException(nameof(tagRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(CreatePrivateTagCommand request, CancellationToken cancellationToken)
        {
            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var tag = new Domain.AggregatesModel.TagAggregate.Tag(request.Name, myId);
            _tagRepository.Add(tag);
            return await _tagRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
