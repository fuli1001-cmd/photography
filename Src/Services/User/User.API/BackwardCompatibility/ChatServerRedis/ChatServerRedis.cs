﻿using Arise.DDD.Infrastructure.Redis;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Photography.Services.User.API.BackwardCompatibility.Models;
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
        private readonly IUserRepository _userRepository;
        private readonly IRedisService _redisService;
        private readonly ILogger<ChatServerRedis> _logger;

        public ChatServerRedis(
            IUserRepository userRepository,
            IRedisService redisService,
            ILogger<ChatServerRedis> logger)
        {
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

        public async Task WriteGroupMemberMessageAsync(Group group, SysMsgType msgType, IEnumerable<Domain.AggregatesModel.UserAggregate.User> changedUsers, Domain.AggregatesModel.UserAggregate.User operatorUser)
        {
            var receivers = await _userRepository.GetUsersAsync(group.GroupUsers.Select(gu => gu.UserId.Value));
            var receiverIds = receivers.Select(u => u.ChatServerUserId).ToArray();

            var changedIds = changedUsers.Select(u => u.ChatServerUserId).ToArray();
            var changedNicknames = changedUsers.Select(u => u.Nickname).ToArray();

            var msg = new SysMsgGroupChangedVo
            {
                type = (int)msgType,
                receiverIds = receiverIds,
                changedMemberIds = changedIds,
                changedMemberNames = changedNicknames,
                groupId = group.ChatServerGroupId,
                operatorName = operatorUser.Nickname,
                operatorId = operatorUser.ChatServerUserId
            };

            _logger.LogInformation("group memeber message: {@msg}", msg);

            await WriteMessageAsync(msg);
        }

        private async Task WriteMessageAsync(SysMsgGroupChangedVo msg)
        {
            var bytesData = SerializeUtil.SerializeToJsonBytes(msg, true);
            string json = JsonConvert.SerializeObject(bytesData);
            await _redisService.PublishAsync("SYS_MSG", json);

            _logger.LogInformation("Redis Message: {@RedisMessage}", msg);
        }

        public async Task WriteUserAsync(Domain.AggregatesModel.UserAggregate.User user)
        {
            var storedUserInfoLite = await GetUserInfoLiteAsync(user.ChatServerUserId);

            var userInfoLite = new UserInfoLite
            {
                userId = user.ChatServerUserId,
                username = user.UserName,
                nickname = user.Nickname,
                clientType = user.ClientType,
                avatar = user.Avatar,
                tel = user.Phonenumber,
                registrationId = user.RegistrationId,
                connectionInfos = storedUserInfoLite?.connectionInfos ?? null
            };

            await SaveUserInfoLiteToRedis(user.ChatServerUserId.ToString(), userInfoLite);
        }

        public async Task RemoveUserAsync(int chatServerUserId, int clientType)
        {
            var storedUserInfoLite = await GetUserInfoLiteAsync(chatServerUserId);

            if (storedUserInfoLite == null)
            {
                await _redisService.KeyDeleteAsync(chatServerUserId.ToString());
            }
            else
            {
                storedUserInfoLite.connectionInfos = storedUserInfoLite.connectionInfos.Where(c => c.ClientType != clientType).ToList();
                await SaveUserInfoLiteToRedis(chatServerUserId.ToString(), storedUserInfoLite);
            }
        }

        private async Task SaveUserInfoLiteToRedis(string key, UserInfoLite userInfoLite)
        {
            string json = SerializeUtil.SerializeToJson(userInfoLite);
            var bytes = SerializeUtil.SerializeStringToBytes(json, true);
            json = JsonConvert.SerializeObject(bytes);

            await _redisService.StringSetAsync(key, json, null);

            _logger.LogInformation("Redis User: {@RedisUser}", userInfoLite);
        }

        public async Task WriteTokenUserAsync(Domain.AggregatesModel.UserAggregate.User user, string oldToken)
        {
            var tokenUser = new Token
            {
                userId = user.ChatServerUserId,
                username = user.UserName,
                nickname = user.Nickname,
                clientType = user.ClientType,
                loginTime = CommonUtil.GetTimestamp(DateTime.Now)
            };

            var bytes = SerializeUtil.SerializeToJsonBytes(tokenUser, true);
            var json = JsonConvert.SerializeObject(bytes);

            await _redisService.StringSetAsync(oldToken, json, null);

            _logger.LogInformation("Redis TokenUser: {@RedisTokenUser}", tokenUser);
        }

        private async Task<UserInfoLite> GetUserInfoLiteAsync(int chatServerUserId)
        {
            try
            {
                var bytesJson = await _redisService.StringGetAsync(chatServerUserId.ToString());
                var bytes = JsonConvert.DeserializeObject<byte[]>(bytesJson);
                var objJson = SerializeUtil.DeserializeBytesToString(bytes, true);
                return SerializeUtil.DeserializeJsonToObject<UserInfoLite>(objJson);
            }
            catch
            {
                return null;
            }
        }

        //private async Task<List<ConnectionInfo>> GetUserConnectionInfoAsync(int chatServerUserId)
        //{
        //    try
        //    {
        //        var bytesJson = await _redisService.StringGetAsync(chatServerUserId.ToString());
        //        var bytes = JsonConvert.DeserializeObject<byte[]>(bytesJson);
        //        var objJson = SerializeUtil.DeserializeBytesToString(bytes, true);
        //        var userInfoLite = SerializeUtil.DeserializeJsonToObject<UserInfoLite>(objJson);
        //        return userInfoLite.connectionInfos;
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}
    }
}
