using Arise.DDD.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Photography.Services.User.Domain.AggregatesModel.GroupAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.Group.UpdateGroupAvatar
{
    public class UpdateGroupAvatarCommandHandler : IRequestHandler<UpdateGroupAvatarCommand, bool>
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UpdateGroupAvatarCommandHandler> _logger;

        public UpdateGroupAvatarCommandHandler(IGroupRepository groupRepository,
            IHttpContextAccessor httpContextAccessor,
            ILogger<UpdateGroupAvatarCommandHandler> logger)
        {
            _groupRepository = groupRepository ?? throw new ArgumentNullException(nameof(groupRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(UpdateGroupAvatarCommand request, CancellationToken cancellationToken)
        {
            var group = await _groupRepository.GetGroupWithMembersAsync(request.GroupId);
            if (group == null)
                throw new ClientException("操作失败", new List<string> { $"Group {request.GroupId} does not exist." });

            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (!group.ModifyMemberEnabled)
            {
                // 允许群成员修改群成员开关没打开，只能允许群主修改
                if (myId != group.OwnerId)
                    throw new ClientException("操作失败", new List<string> { $"Group {request.GroupId} does not belong to user {myId}." });
            }
            else if (!group.GroupUsers.Any(gu => gu.UserId == myId))
            {
                // 允许群成员修改群成员开关已打开，允许群成员修改，检查是否是群成员
                throw new ClientException("操作失败", new List<string> { $"User {myId} is not in Group {request.GroupId}." });
            }

            group.Update(group.Name, group.Notice, request.Avatar);

            if (await _groupRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
                return true;

            throw new ApplicationException("操作失败");
        }
    }
}
