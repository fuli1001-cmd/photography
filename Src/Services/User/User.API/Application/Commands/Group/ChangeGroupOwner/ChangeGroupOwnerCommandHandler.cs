using Arise.DDD.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
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

namespace Photography.Services.User.API.Application.Commands.Group.ChangeGroupOwner
{
    public class ChangeGroupOwnerCommandHandler : IRequestHandler<ChangeGroupOwnerCommand, bool>
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IUserRepository _userRepository;
        private readonly IChatServerRedis _chatServerRedisService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ChangeGroupOwnerCommandHandler> _logger;

        public ChangeGroupOwnerCommandHandler(IGroupRepository groupRepository,
            IUserRepository userRepository,
            IChatServerRedis chatServerRedisService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ChangeGroupOwnerCommandHandler> logger)
        {
            _groupRepository = groupRepository ?? throw new ArgumentNullException(nameof(groupRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _chatServerRedisService = chatServerRedisService ?? throw new ArgumentNullException(nameof(chatServerRedisService));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ChangeGroupOwnerCommand request, CancellationToken cancellationToken)
        {
            var group = await _groupRepository.GetGroupWithMembersAsync(request.GroupId);
            if (group == null)
                throw new ClientException("操作失败", new List<string> { $"Group {request.GroupId} does not exist." });

            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            group.ChangeOwner(myId, request.NewOwnerId);

            if (await _groupRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
            {
                // BackwardCompatibility: 为了兼容以前的聊天服务，需要向redis写入相关数据
                await UpdateRedisAsync(request, group);

                return true;
            }

            throw new ApplicationException("操作失败");
        }

        #region BackwardCompatibility: 为了兼容以前的聊天服务，需要向redis写入相关数据
        private async Task UpdateRedisAsync(ChangeGroupOwnerCommand request, Domain.AggregatesModel.GroupAggregate.Group group)
        {
            try
            {
                // 向redis写入群
                await _chatServerRedisService.WriteGroupAsync(group);

                // 发布系统消息
                var changedUsers = await _userRepository.GetUsersAsync(new List<Guid> { request.NewOwnerId });
                await _chatServerRedisService.WriteGroupMemberMessageAsync(group, SysMsgType.OWNER_CHANGED, changedUsers);
            }
            catch (Exception ex)
            {
                _logger.LogError("Redis Error: {@RedisError}", ex);
            }
        }
        #endregion
    }
}
