using Arise.DDD.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Photography.Services.User.API.BackwardCompatibility.Models;
using Photography.Services.User.API.Infrastructure.Redis;
using Photography.Services.User.Domain.AggregatesModel.GroupAggregate;
using Photography.Services.User.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using UtilLib.Util;

namespace Photography.Services.User.API.Application.Commands.Group.UpdateGroup
{
    public class UpdateGroupCommandHandler : IRequestHandler<UpdateGroupCommand, bool>
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRedisService _redisService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UpdateGroupCommandHandler> _logger;

        public UpdateGroupCommandHandler(IGroupRepository groupRepository,
            IUserRepository userRepository,
            IRedisService redisService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<UpdateGroupCommandHandler> logger)
        {
            _groupRepository = groupRepository ?? throw new ArgumentNullException(nameof(groupRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _redisService = redisService ?? throw new ArgumentNullException(nameof(redisService));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
        {
            var group = await _groupRepository.GetByIdAsync(request.GroupId);
            if (group == null)
            {
                _logger.LogError("UpdateGroupCommandHandler: Group {GroupId} does not exist.", request.GroupId);
                throw new DomainException("操作失败。");
            }

            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (myId != group.OwnerId)
            {
                _logger.LogError("UpdateGroupCommandHandler: Group {GroupId} does not belong to user {UserId}.", request.GroupId, myId);
                throw new DomainException("操作失败。");
            }

            group.Update(request.Name, request.Notice, request.Avatar);

            if (await _groupRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
            {
                // BackwardCompatibility: 为了兼容以前的聊天服务，需要向redis写入相关数据
                await UpdateRedisAsync(group);

                return true;
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

        private async Task WriteMessageToRedisAsync(Domain.AggregatesModel.GroupAggregate.Group group, Domain.AggregatesModel.UserAggregate.User owner)
        {
            var receiverIds = (await _userRepository.GetUsersAsync(group.GroupUsers.Select(gu => gu.UserId.Value))).Select(u => u.ChatServerUserId).ToArray();

            var msg = new SysMsgGroupChangedVo
            {
                type = (int)SysMsgType.GROUP_INFO_CHANGED,
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
