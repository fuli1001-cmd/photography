using Arise.DDD.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Photography.Services.User.API.BackwardCompatibility.Models;
using Photography.Services.User.API.Infrastructure.Redis;
using Photography.Services.User.API.Query.Interfaces;
using Photography.Services.User.API.Query.ViewModels;
using Photography.Services.User.Domain.AggregatesModel.GroupAggregate;
using Photography.Services.User.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using UtilLib.Util;

namespace Photography.Services.User.API.Application.Commands.Group.CreateGroup
{
    public class CreateGroupCommandHandler : IRequestHandler<CreateGroupCommand, GroupViewModel>
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IGroupQueries _groupQueries;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRedisService _redisService;
        private readonly ILogger<CreateGroupCommandHandler> _logger;

        public CreateGroupCommandHandler(IGroupRepository groupRepository,
            IGroupQueries groupQueries,
            IUserRepository userRepository,
            IHttpContextAccessor httpContextAccessor,
            IRedisService redisService,
            ILogger<CreateGroupCommandHandler> logger)
        {
            _groupRepository = groupRepository ?? throw new ArgumentNullException(nameof(groupRepository));
            _groupQueries = groupQueries ?? throw new ArgumentNullException(nameof(groupQueries));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _redisService = redisService ?? throw new ArgumentNullException(nameof(redisService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<GroupViewModel> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
        {
            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var group = new Domain.AggregatesModel.GroupAggregate.Group(request.Name, request.Avatar, myId, request.MemberIds);
            _groupRepository.Add(group);

            if (await _groupRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
            {
                // BackwardCompatibility: 为了兼容以前的聊天服务，需要向redis写入相关数据
                await UpdateRedisAsync(group);

                return await _groupQueries.GetGroupAsync(group.Id);
            }

            throw new DomainException("操作失败。");
        }

        #region BackwardCompatibility: 为了兼容以前的聊天服务，需要向redis写入相关数据
        private async Task UpdateRedisAsync(Domain.AggregatesModel.GroupAggregate.Group group)
        {
            try
            {
                var owner = await _userRepository.GetByIdAsync(group.OwnerId);

                // 向redis写入群
                await WriteGroupToRedisAsync(group, owner);

                // 向redis写入群成员
                foreach (var groupUser in group.GroupUsers)
                {
                    await WriteGroupMemberToRedisAsync(groupUser.UserId.Value, group.ChatServerGroupId);
                }

                // 发布系统消息
                await WriteMessageToRedisAsync(group, owner);
            }
            catch (Exception ex)
            {
                _logger.LogError("CreateGroupCommandHandler UpdateRedisAsync: {@BackwardCompatibilityError}", ex);
            }
        }

        private async Task WriteGroupToRedisAsync(Domain.AggregatesModel.GroupAggregate.Group group, Domain.AggregatesModel.UserAggregate.User owner)
        {
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
                IMARSG_AllowAddMemberByAnyone = 0
            };

            var groupBytesData = SerializeUtil.SerializeToJsonBytes(chatServerGroup, true);
            string json = JsonConvert.SerializeObject(groupBytesData);
            await _redisService.HashSetAsync("grouptable", group.ChatServerGroupId.ToString(), json);

            _logger.LogInformation("Redis Group: {@RedisGroup}", chatServerGroup);
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
            await _redisService.HashSetAsync("group_" + member.IMARSGM_GroupId, user.ChatServerUserId.ToString(), json);

            _logger.LogInformation("Redis GroupMember: {@RedisGroupMember}", member);
        }

        private async Task WriteMessageToRedisAsync(Domain.AggregatesModel.GroupAggregate.Group group, Domain.AggregatesModel.UserAggregate.User owner)
        {
            // write message to redis
            var receiverIds = (await _userRepository.GetUsersAsync(group.GroupUsers.Select(gu => gu.UserId.Value))).Select(u => u.ChatServerUserId).ToArray();

            var msg = new SysMsgGroupChangedVo
            {
                type = (int)SysMsgType.GROUP_CREATED,
                receiverIds = receiverIds,
                groupId = group.ChatServerGroupId,
                operatorName = owner.Nickname,
                operatorId = owner.ChatServerUserId
            };
            
            var bytesData = SerializeUtil.SerializeToJsonBytes(msg, true);
            string json = JsonConvert.SerializeObject(bytesData);
            await _redisService.PublishAsync("SYS_MSG", json);

            _logger.LogInformation("Redis Message: {@RedisMessage}", msg);
        }
        #endregion
    }
}
