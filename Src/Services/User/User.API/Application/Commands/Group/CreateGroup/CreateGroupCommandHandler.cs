using Arise.DDD.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Photography.Services.User.API.BackwardCompatibility.ChatServerRedis;
using Photography.Services.User.API.BackwardCompatibility.Models;
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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IChatServerRedis _chatServerRedisService;
        private readonly ILogger<CreateGroupCommandHandler> _logger;

        public CreateGroupCommandHandler(IGroupRepository groupRepository,
            IGroupQueries groupQueries,
            IHttpContextAccessor httpContextAccessor,
            IChatServerRedis chatServerRedisService,
            ILogger<CreateGroupCommandHandler> logger)
        {
            _groupRepository = groupRepository ?? throw new ArgumentNullException(nameof(groupRepository));
            _groupQueries = groupQueries ?? throw new ArgumentNullException(nameof(groupQueries));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _chatServerRedisService = chatServerRedisService ?? throw new ArgumentNullException(nameof(chatServerRedisService));
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

                return await _groupQueries.GetGroupAsync(group.Id, null);
            }

            throw new ApplicationException("操作失败");
        }

        #region BackwardCompatibility: 为了兼容以前的聊天服务，需要向redis写入相关数据
        private async Task UpdateRedisAsync(Domain.AggregatesModel.GroupAggregate.Group group)
        {
            try
            {
                // 向redis写入群
                await _chatServerRedisService.WriteGroupAsync(group);

                // 向redis写入群成员
                foreach (var groupUser in group.GroupUsers)
                {
                    await _chatServerRedisService.WriteGroupMemberAsync(groupUser.UserId.Value, group.ChatServerGroupId, 0);
                }

                // 发布系统消息
                await _chatServerRedisService.WriteGroupMessageAsync(group, SysMsgType.GROUP_CREATED);
            }
            catch (Exception ex)
            {
                _logger.LogError("Redis Error: {@RedisError}", ex);
            }
        }
        #endregion
    }
}
