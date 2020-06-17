using Arise.DDD.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Photography.Services.User.API.BackwardCompatibility.ChatServerRedis;
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
        private readonly IChatServerRedis _chatServerRedisService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ModifyGroupMembersCommandHandler> _logger;

        public ModifyGroupMembersCommandHandler(IGroupRepository groupRepository,
            IGroupUserRepository groupUserRepository,
            IUserRepository userRepository,
            IChatServerRedis chatServerRedisService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ModifyGroupMembersCommandHandler> logger)
        {
            _groupRepository = groupRepository ?? throw new ArgumentNullException(nameof(groupRepository));
            _groupUserRepository = groupUserRepository ?? throw new ArgumentNullException(nameof(groupUserRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _chatServerRedisService = chatServerRedisService ?? throw new ArgumentNullException(nameof(chatServerRedisService));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ModifyGroupMembersCommand request, CancellationToken cancellationToken)
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

            // 删除需要删除的成员
            request.RemovedMemberIds.ForEach(memberId =>
            {
                var groupUser = group.GroupUsers.SingleOrDefault(gu => gu.UserId == memberId);
                if (groupUser != null)
                    _groupUserRepository.Remove(groupUser);
            });

            // 添加新成员
            request.NewMemberIds.ForEach(memberId => _groupUserRepository.Add(new GroupUser(request.GroupId, memberId)));

            if (await _groupUserRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
            {
                // BackwardCompatibility: 为了兼容以前的聊天服务，需要向redis写入相关数据
                await UpdateRedisAsync(request, group);

                return true;
            }

            throw new ApplicationException("操作失败");
        }

        #region BackwardCompatibility: 为了兼容以前的聊天服务，需要向redis写入相关数据
        private async Task UpdateRedisAsync(ModifyGroupMembersCommand request, Domain.AggregatesModel.GroupAggregate.Group group)
        {
            try
            {
                // 从redis去掉被删除的群成员
                foreach (var memberId in request.RemovedMemberIds)
                {
                    await _chatServerRedisService.RemoveGroupMemberAsync(memberId, group.ChatServerGroupId);
                }

                // 向redis加入新增的群成员
                foreach (var memberId in request.NewMemberIds)
                {
                    await _chatServerRedisService.WriteGroupMemberAsync(memberId, group.ChatServerGroupId, 0);
                }

                // 发布系统消息
                var removedUsers = await _userRepository.GetUsersAsync(request.RemovedMemberIds);
                await _chatServerRedisService.WriteGroupMemberMessageAsync(group, SysMsgType.REMOVED_FROM_GROUP, removedUsers);

                var addedUsers = await _userRepository.GetUsersAsync(request.NewMemberIds);
                await _chatServerRedisService.WriteGroupMemberMessageAsync(group, SysMsgType.NEW_MEMBER_ADDED, removedUsers);
            } 
            catch (Exception ex)
            {
                _logger.LogError("Redis Error: {@RedisError}", ex);
            }
        }
        #endregion
    }
}
