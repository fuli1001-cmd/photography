using Arise.DDD.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Photography.Services.User.API.BackwardCompatibility.ChatServerRedis;
using Photography.Services.User.API.BackwardCompatibility.Models;
using Photography.Services.User.Domain.AggregatesModel.GroupAggregate;
using Photography.Services.User.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using UtilLib.Util;

namespace Photography.Services.User.API.Application.Commands.Group.DeleteGroup
{
    public class DeleteGroupCommandHandler : IRequestHandler<DeleteGroupCommand, bool>
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IChatServerRedis _chatServerRedisService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<DeleteGroupCommandHandler> _logger;

        public DeleteGroupCommandHandler(IGroupRepository groupRepository,
            IChatServerRedis chatServerRedisService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<DeleteGroupCommandHandler> logger)
        {
            _groupRepository = groupRepository ?? throw new ArgumentNullException(nameof(groupRepository));
            _chatServerRedisService = chatServerRedisService ?? throw new ArgumentNullException(nameof(chatServerRedisService));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
        {
            var group = await _groupRepository.GetGroupWithMembersAsync(request.GroupId);
            if (group == null)
                throw new ClientException("操作失败", new List<string> { $"Group {request.GroupId} does not exist." });

            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (myId != group.OwnerId)
                throw new ClientException("操作失败", new List<string> { $"Group {request.GroupId} does not belong to user {myId}." });

            //var memberIds = group.GroupUsers.Select(gu => gu.UserId.Value).ToList();

            group.Delete();

            _groupRepository.Remove(group);

            if (await _groupRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
            {
                _logger.LogInformation("group after delete: {@group}", group);
                // BackwardCompatibility: 为了兼容以前的聊天服务，需要向redis写入相关数据
                await UpdateRedisAsync(group);

                return true;
            }

            throw new ApplicationException("操作失败");
        }

        #region BackwardCompatibility: 为了兼容以前的聊天服务，需要向redis写入相关数据
        private async Task UpdateRedisAsync(Domain.AggregatesModel.GroupAggregate.Group group)
        {
            try
            {
                // 从redis去除group
                await _chatServerRedisService.RemoveGroupAsync(group.ChatServerGroupId);

                // 从redis去除group members
                var userIds = group.GroupUsers.Select(gu => gu.UserId.Value);
                foreach (var userId in userIds)
                { 
                    await _chatServerRedisService.RemoveGroupMemberAsync(userId, group.ChatServerGroupId);
                }

                // 发布系统消息
                await _chatServerRedisService.WriteGroupMessageAsync(group, SysMsgType.GROUP_DISMISSED);
            }
            catch (Exception ex)
            {
                _logger.LogError("Redis Error: {@RedisError}", ex);
            }
        }
        #endregion
    }
}
