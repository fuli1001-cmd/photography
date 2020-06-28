using Arise.DDD.Domain.Exceptions;
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

namespace Photography.Services.Post.API.Application.Commands.Tag.DeletePrivateTag
{
    public class DeletePrivateTagCommandHandler : IRequestHandler<DeletePrivateTagCommand, bool>
    {
        private readonly ITagRepository _tagRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<DeletePrivateTagCommandHandler> _logger;

        public DeletePrivateTagCommandHandler(ITagRepository tagRepository,
            IHttpContextAccessor httpContextAccessor,
            ILogger<DeletePrivateTagCommandHandler> logger)
        {
            _tagRepository = tagRepository ?? throw new ArgumentNullException(nameof(tagRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(DeletePrivateTagCommand request, CancellationToken cancellationToken)
        {
            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var tag = await _tagRepository.GetUserPrivateTagByName(myId, request.Name);

            if (tag == null)
                throw new ClientException("操作失败", new List<string> { $"Tag {request.Name} does not belong to user {myId}" });

            tag.Delete();
            _tagRepository.Remove(tag);

            return await _tagRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
