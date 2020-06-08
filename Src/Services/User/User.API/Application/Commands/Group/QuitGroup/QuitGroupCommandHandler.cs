using Arise.DDD.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Photography.Services.User.Domain.AggregatesModel.GroupUserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.Group.QuitGroup
{
    public class QuitGroupCommandHandler : IRequestHandler<QuitGroupCommand, bool>
    {
        private readonly IGroupUserRepository _groupUserRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<QuitGroupCommandHandler> _logger;

        public QuitGroupCommandHandler(IGroupUserRepository groupUserRepository,
            IHttpContextAccessor httpContextAccessor,
            ILogger<QuitGroupCommandHandler> logger)
        {
            _groupUserRepository = groupUserRepository ?? throw new ArgumentNullException(nameof(groupUserRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(QuitGroupCommand request, CancellationToken cancellationToken)
        {
            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var groupUser = await _groupUserRepository.GetGroupUserAsync(request.GroupId, myId);

            if (groupUser == null)
            {
                _logger.LogError("QuitGroupCommandHandler: User {UserId} is not in Group {GroupId}.", myId, request.GroupId);
                throw new DomainException("操作失败。");
            }

            _groupUserRepository.Remove(groupUser);

            return await _groupUserRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
