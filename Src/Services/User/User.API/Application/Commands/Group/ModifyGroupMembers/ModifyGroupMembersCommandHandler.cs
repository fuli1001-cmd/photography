using Arise.DDD.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Photography.Services.User.Domain.AggregatesModel.GroupAggregate;
using Photography.Services.User.Domain.AggregatesModel.GroupUserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.Group.ModifyGroupMembers
{
    public class ModifyGroupMembersCommandHandler : IRequestHandler<ModifyGroupMembersCommand, bool>
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IGroupUserRepository _groupUserRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ModifyGroupMembersCommandHandler> _logger;

        public ModifyGroupMembersCommandHandler(IGroupRepository groupRepository,
            IGroupUserRepository groupUserRepository,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ModifyGroupMembersCommandHandler> logger)
        {
            _groupRepository = groupRepository ?? throw new ArgumentNullException(nameof(groupRepository));
            _groupUserRepository = groupUserRepository ?? throw new ArgumentNullException(nameof(groupUserRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ModifyGroupMembersCommand request, CancellationToken cancellationToken)
        {
            var group = await _groupRepository.GetGroupWithMembersAsync(request.GroupId);
            if (group == null)
            {
                _logger.LogError("ModifyGroupMembersCommandHandler: Group {GroupId} does not exist.", request.GroupId);
                throw new DomainException("操作失败。");
            }

            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (!group.ModifyMemberEnabled)
            {
                // 允许群成员修改群成员开关没打开，只能允许群主修改
                if (myId != group.OwnerId)
                {
                    _logger.LogError("ModifyGroupMembersCommandHandler: Group {GroupId} does not belong to user {UserId}.", request.GroupId, myId);
                    throw new DomainException("操作失败。");
                }
            }
            else if (!group.GroupUsers.Any(gu => gu.UserId == myId))
            {
                // 允许群成员修改群成员开关已打开，允许群成员修改
                _logger.LogError("ModifyGroupMembersCommandHandler: User {UserId} is not in Group {GroupId}.", myId, request.GroupId);
                throw new DomainException("操作失败。");
            }

            // 删除需要删除的成员
            request.RemovedMemberIds.ForEach(memberId =>
            {
                var groupUser = group.GroupUsers.SingleOrDefault(gu => gu.UserId == memberId);
                if (groupUser != null)
                    _groupUserRepository.Remove(groupUser);
            });

            // 增加需要增加的成员
            group.AddMembers(request.NewMemberIds);

            return await _groupRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
