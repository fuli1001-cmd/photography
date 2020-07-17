using Arise.DDD.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _configuration;

        public CreatePrivateTagCommandHandler(ITagRepository tagRepository,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration,
            ILogger<CreatePrivateTagCommandHandler> logger)
        {
            _tagRepository = tagRepository ?? throw new ArgumentNullException(nameof(tagRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(CreatePrivateTagCommand request, CancellationToken cancellationToken)
        {
            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            // 同一用户不能创建重复名称的类别
            var tag = await _tagRepository.GetUserPrivateTagByName(myId, request.Name);
            if (tag != null)
                throw new ClientException("相册分类已存在");

            var privateTagCount = await _tagRepository.GetUserPrivateTagCount(myId);
            if (privateTagCount >= _configuration.GetValue("MaxPrivateTagCount", 10))
                throw new ClientException("相册分类数量已达上限");

            tag = new Domain.AggregatesModel.TagAggregate.Tag(request.Name, myId);
            _tagRepository.Add(tag);

            return await _tagRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
