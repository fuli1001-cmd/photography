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
                await WriteGroupToRedisAsync(group);

                return await _groupQueries.GetGroupAsync(group.Id);
            }

            _logger.LogError("CreateGroupCommandHandler: Create group {@CreateGroupCommand} failed.", request);
            throw new DomainException("操作失败。");
        }

        #region BackwardCompatibility: 为了兼容以前的聊天服务，需要向redis写入相关数据
        private async Task WriteGroupToRedisAsync(Domain.AggregatesModel.GroupAggregate.Group group)
        {
            try
            {
                // write group to redis
                var owner = await _userRepository.GetByIdAsync(group.OwnerId);

                var chatServerGroup = new PSR_ARS_Group
                {
                    IMARSG_CreateTime = DateTime.Now,
                    IMARSG_MembersNum = group.GroupUsers.Count,
                    IMARSG_Name = group.Name,
                    IMARSG_Notice = string.Empty,
                    IMARSG_OwnerId = owner.ChatServerUserId,
                    IMARSG_Status = 1,
                    IMARSG_Type = 1,
                    IMARSG_Avatar = group.Avatar,
                    IMARSG_Guid = CommonUtil.GetGuid(),
                    IMARSG_AllowAddMemberByAnyone = 0
                };
                _logger.LogInformation("PSR_ARS_Group: {@PSR_ARS_Group}", chatServerGroup);
                var groupBytesData = SerializeUtil.SerializeToJsonBytes(chatServerGroup, true);
                string json = JsonConvert.SerializeObject(groupBytesData);
                await _redisService.HashSetAsync("grouptable", group.ChatServerGroupId.ToString(), json);

                // write group members to redis
                foreach(var groupUser in group.GroupUsers)
                {
                    await WriteGroupMemberToRedisAsync(groupUser.UserId.Value, group.ChatServerGroupId);
                }

                // write message to redis 
                await WriteMessageToRedisAsync(owner, group);
            }
            catch (Exception ex)
            {
                _logger.LogError("WriteGroupToRedisAsync: {BackwardCompatibilityError}", ex.Message);
                if (ex.InnerException != null)
                    _logger.LogError("WriteGroupToRedisAsync: {BackwardCompatibilityError}", ex.InnerException.Message);
            }
        }

        private async Task WriteGroupMemberToRedisAsync(Guid userId, int chatServerGroupId)
        {
            try
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
                _logger.LogInformation("PSR_ARS_GroupMembers: {@PSR_ARS_Group}", member);
                var bytesData = SerializeUtil.SerializeToJsonBytes(member, true);
                string json = JsonConvert.SerializeObject(bytesData);
                await _redisService.HashSetAsync("group_" + member.IMARSGM_GroupId, user.ChatServerUserId.ToString(), json);
            }
            catch (Exception ex)
            {
                _logger.LogError("WriteGroupMemberToRedisAsync: {BackwardCompatibilityError}", ex.Message);
                if (ex.InnerException != null)
                    _logger.LogError("WriteGroupMemberToRedisAsync: {BackwardCompatibilityError}", ex.InnerException.Message);
            }
        }

        private async Task WriteMessageToRedisAsync(Domain.AggregatesModel.UserAggregate.User owner, Domain.AggregatesModel.GroupAggregate.Group group)
        {
            try
            {
                // write message to redis
                var chatServerUserIds = await _userRepository.GetChatServerUserIdsAsync(group.GroupUsers.Select(gu => gu.UserId.Value));

                var sysMsgVo = new SysMsgGroupChangedVo
                {
                    type = (int)SysMsgType.GROUP_CREATED,
                    receiverIds = chatServerUserIds,
                    groupId = group.ChatServerGroupId,
                    operatorName = owner.Nickname,
                    operatorId = owner.ChatServerUserId
                };
                _logger.LogInformation("SysMsgGroupChangedVo: {@SysMsgGroupChangedVo}", sysMsgVo);
                var bytesData = SerializeUtil.SerializeToJsonBytes(sysMsgVo, true);
                string json = JsonConvert.SerializeObject(bytesData);
                await _redisService.PublishAsync("SYS_MSG", json);
            }
            catch (Exception ex)
            {
                _logger.LogError("WriteMessageToRedisAsync: {BackwardCompatibilityError}", ex.Message);
                if (ex.InnerException != null)
                    _logger.LogError("WriteMessageToRedisAsync: {BackwardCompatibilityError}", ex.InnerException.Message);
            }
        }
        #endregion
    }
}
