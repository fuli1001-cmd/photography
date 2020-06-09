using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Photography.Services.User.API.BackwardCompatibility.Models;
using Photography.Services.User.API.Infrastructure.Redis;
using Photography.Services.User.Domain.AggregatesModel.GroupAggregate;
using Photography.Services.User.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UtilLib.Util;

namespace Photography.Services.User.API.BackwardCompatibility.ChatServerRedis
{
    public class ChatServerRedis : IChatServerRedis
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRedisService _redisService;
        private readonly ILogger<ChatServerRedis> _logger;

        public ChatServerRedis(
            IGroupRepository groupRepository,
            IUserRepository userRepository,
            IRedisService redisService,
            ILogger<ChatServerRedis> logger)
        {
            _groupRepository = groupRepository ?? throw new ArgumentNullException(nameof(groupRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _redisService = redisService ?? throw new ArgumentNullException(nameof(redisService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task WriteGroupAsync(Group group)
        {
            var owner = await _userRepository.GetByIdAsync(group.OwnerId);

            var chatServerGroup = new PSR_ARS_Group
            {
                IMARSG_CreateTime = DateTime.UnixEpoch.AddSeconds(group.CreatedTime),
                IMARSG_MembersNum = group.GroupUsers.Count,
                IMARSG_Name = group.Name,
                IMARSG_Avatar = group.Avatar,
                IMARSG_Notice = group.Notice,
                IMARSG_OwnerId = owner.ChatServerUserId,
                IMARSG_Status = 1,
                IMARSG_Type = 1,
                IMARSG_Guid = CommonUtil.GetGuid(),
                IMARSG_AllowAddMemberByAnyone = group.ModifyMemberEnabled ? 1 : 0
            };

            var groupBytesData = SerializeUtil.SerializeToJsonBytes(chatServerGroup, true);
            string json = JsonConvert.SerializeObject(groupBytesData);
            await _redisService.HashSetAsync("grouptable", group.ChatServerGroupId.ToString(), json);

            _logger.LogInformation("Redis Group: {@RedisGroup}", chatServerGroup);
        }

        public async Task WriteGroupMemberAsync(Guid userId, int chatServerGroupId, int muted)
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
                IMARSGM_Mute = muted
            };

            var bytesData = SerializeUtil.SerializeToJsonBytes(member, true);
            string json = JsonConvert.SerializeObject(bytesData);
            await _redisService.HashSetAsync("group_" + member.IMARSGM_GroupId, user.ChatServerUserId.ToString(), json);

            _logger.LogInformation("Redis GroupMember: {@RedisGroupMember}", member);
        }

        public async Task RemoveGroupAsync(int chatServerGroupId)
        {
            await _redisService.HashDeleteAsync("grouptable", chatServerGroupId.ToString());
        }

        public async Task RemoveGroupMemberAsync(Guid userId, int chatServerGroupId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            await _redisService.HashDeleteAsync("group_" + chatServerGroupId, user.ChatServerUserId.ToString());
        }

        public async Task WriteGroupMessageAsync(Group group, SysMsgType msgType)
        {
            var owner = await _userRepository.GetByIdAsync(group.OwnerId);
            var receiverIds = (await _userRepository.GetUsersAsync(group.GroupUsers.Select(gu => gu.UserId.Value))).Select(u => u.ChatServerUserId).ToArray();

            var msg = new SysMsgGroupChangedVo
            {
                type = (int)msgType,
                receiverIds = receiverIds,
                groupId = group.ChatServerGroupId,
                operatorName = owner.Nickname,
                operatorId = owner.ChatServerUserId
            };

            await WriteMessageAsync(msg);
        }

        public async Task WriteGroupMemberMessageAsync(Group group, SysMsgType msgType, IEnumerable<Domain.AggregatesModel.UserAggregate.User> changedUsers)
        {
            var owner = await _userRepository.GetByIdAsync(group.OwnerId);
            var receiverIds = (await _userRepository.GetUsersAsync(group.GroupUsers.Select(gu => gu.UserId.Value))).Select(u => u.ChatServerUserId).ToArray();

            var changedIds = changedUsers.Select(u => u.ChatServerUserId).ToArray();
            var changedNicknames = changedUsers.Select(u => u.Nickname).ToArray();

            var removedMsg = new SysMsgGroupChangedVo
            {
                type = (int)msgType,
                receiverIds = receiverIds,
                changedMemberIds = changedIds,
                changedMemberNames = changedNicknames,
                groupId = group.ChatServerGroupId,
                operatorName = owner.Nickname,
                operatorId = owner.ChatServerUserId
            };

            await WriteMessageAsync(removedMsg);
        }

        private async Task WriteMessageAsync(SysMsgGroupChangedVo msg)
        {
            var bytesData = SerializeUtil.SerializeToJsonBytes(msg, true);
            string json = JsonConvert.SerializeObject(bytesData);
            await _redisService.PublishAsync("SYS_MSG", json);

            _logger.LogInformation("Redis Message: {@RedisMessage}", msg);
        }
    }
}
