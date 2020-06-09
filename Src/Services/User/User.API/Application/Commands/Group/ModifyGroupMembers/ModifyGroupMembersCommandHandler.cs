using Arise.DDD.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Photography.Services.User.API.BackwardCompatibility.Models;
using Photography.Services.User.API.Infrastructure.Redis;
using Photography.Services.User.Domain.AggregatesModel.GroupAggregate;
using Photography.Services.User.Domain.AggregatesModel.GroupUserAggregate;
using Photography.Services.User.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using UtilLib.Util;

namespace Photography.Services.User.API.Application.Commands.Group.ModifyGroupMembers
{
    public class ModifyGroupMembersCommandHandler : IRequestHandler<ModifyGroupMembersCommand, bool>
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IGroupUserRepository _groupUserRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRedisService _redisService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ModifyGroupMembersCommandHandler> _logger;

        public ModifyGroupMembersCommandHandler(IGroupRepository groupRepository,
            IGroupUserRepository groupUserRepository,
            IUserRepository userRepository,
            IRedisService redisService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ModifyGroupMembersCommandHandler> logger)
        {
            _groupRepository = groupRepository ?? throw new ArgumentNullException(nameof(groupRepository));
            _groupUserRepository = groupUserRepository ?? throw new ArgumentNullException(nameof(groupUserRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _redisService = redisService ?? throw new ArgumentNullException(nameof(redisService));
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

            if (await _groupRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
            {
                // BackwardCompatibility: 为了兼容以前的聊天服务，需要向redis写入相关数据
                await UpdateRedisAsync(request, group);

                return true;
            }

            throw new DomainException("操作失败。");
        }

        #region BackwardCompatibility: 为了兼容以前的聊天服务，需要向redis写入相关数据
        private async Task UpdateRedisAsync(ModifyGroupMembersCommand request, Domain.AggregatesModel.GroupAggregate.Group group)
        {
            try
            {
                // 从redis去掉被删除的群成员
                request.RemovedMemberIds.ForEach(async memberId =>
                {
                    await RemoveGroupMemberFromRedisAsync(memberId, group.ChatServerGroupId);
                });

                // 向redis加入新增的群成员
                request.NewMemberIds.ForEach(async memberId =>
                {
                    await WriteGroupMemberToRedisAsync(memberId, group.ChatServerGroupId);
                });

                // 发布系统消息
                await WriteMessageToRedisAsync(request, group);
            } 
            catch (Exception ex)
            {
                _logger.LogError("ModifyGroupMembersCommandHandler UpdateRedisAsync: {@BackwardCompatibilityError}", ex);
            }
        }

        private async Task RemoveGroupMemberFromRedisAsync(Guid userId, int chatServerGroupId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            await _redisService.HashDeleteAsync("group_" + chatServerGroupId, user.ChatServerUserId.ToString());
        }

        private async Task WriteGroupMemberToRedisAsync(Guid userId, int chatServerGroupId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            var member = new PSR_ARS_GroupMembers
            {
                IMARSGM_GroupId = chatServerGroupId,
                IMARSGM_AddMember = 1,
                IMARSGM_GroupStatus = 1,
                IMARSGM_MemberId = user.ChatServerUserId,
                IMARSGM_Nickname = user.Nickname,
                IMARSGM_Speaking = 1,
                IMARSGM_Time = DateTime.Now,
                IMARSGM_Mute = 0
            };
            
            var bytesData = SerializeUtil.SerializeToJsonBytes(member, true);
            string json = JsonConvert.SerializeObject(bytesData);
            await _redisService.HashSetAsync("group_" + chatServerGroupId, user.ChatServerUserId.ToString(), json);

            _logger.LogInformation("Redis GroupMember: {@RedisGroupMember}", member);
        }

        private async Task WriteMessageToRedisAsync(ModifyGroupMembersCommand request, Domain.AggregatesModel.GroupAggregate.Group group)
        {
            var owner = await _userRepository.GetByIdAsync(group.OwnerId);

            var receiverIds = (await _userRepository.GetUsersAsync(group.GroupUsers.Select(gu => gu.UserId.Value))).Select(u => u.ChatServerUserId).ToArray();

            // 写入删除群成员消息
            var removedUsers = await _userRepository.GetUsersAsync(request.RemovedMemberIds);
            var removedIds = removedUsers.Select(u => u.ChatServerUserId).ToArray();
            var removedNicknames = removedUsers.Select(u => u.Nickname).ToArray();
            
            var removedMsg = new SysMsgGroupChangedVo
            {
                type = (int)SysMsgType.REMOVED_FROM_GROUP,
                receiverIds = receiverIds,
                changedMemberIds = removedIds,
                changedMemberNames = removedNicknames,
                groupId = group.ChatServerGroupId,
                operatorName = owner.Nickname,
                operatorId = owner.ChatServerUserId
            };

            await WriteMessageToRedisAsync(removedMsg);

            // 写入新增群成员消息
            var addedUsers = await _userRepository.GetUsersAsync(request.NewMemberIds);
            var addedUserIds = addedUsers.Select(u => u.ChatServerUserId).ToArray();
            var addedNicknames = addedUsers.Select(u => u.Nickname).ToArray();

            var addedMsg = new SysMsgGroupChangedVo
            {
                type = (int)SysMsgType.NEW_MEMBER_ADDED,
                receiverIds = receiverIds,
                changedMemberIds = addedUserIds,
                changedMemberNames = addedNicknames,
                groupId = group.ChatServerGroupId,
                operatorName = owner.Nickname,
                operatorId = owner.ChatServerUserId
            };

            await WriteMessageToRedisAsync(addedMsg);
        }

        private async Task WriteMessageToRedisAsync(SysMsgGroupChangedVo msg)
        {
            var bytesData = SerializeUtil.SerializeToJsonBytes(msg, true);
            string json = JsonConvert.SerializeObject(bytesData);
            await _redisService.PublishAsync("SYS_MSG", json);

            _logger.LogInformation("Redis Message: {@RedisMessage}", msg);
        }
        #endregion
    }
}
