using Arise.DDD.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Photography.Services.User.API.BackwardCompatibility.ChatServerRedis;
using Photography.Services.User.API.BackwardCompatibility.Models;
using Photography.Services.User.Domain.AggregatesModel.GroupAggregate;
using Photography.Services.User.Domain.AggregatesModel.GroupUserAggregate;
using Photography.Services.User.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.Group.QuitGroup
{
    public class QuitGroupCommandHandler : IRequestHandler<QuitGroupCommand, bool>
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IGroupUserRepository _groupUserRepository;
        private readonly IUserRepository _userRepository;
        private readonly IChatServerRedis _chatServerRedisService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<QuitGroupCommandHandler> _logger;

        public QuitGroupCommandHandler(
            IGroupRepository groupRepository, 
            IGroupUserRepository groupUserRepository,
            IUserRepository userRepository,
            IChatServerRedis chatServerRedisService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<QuitGroupCommandHandler> logger)
        {
            _groupRepository = groupRepository ?? throw new ArgumentNullException(nameof(groupRepository));
            _groupUserRepository = groupUserRepository ?? throw new ArgumentNullException(nameof(groupUserRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _chatServerRedisService = chatServerRedisService ?? throw new ArgumentNullException(nameof(chatServerRedisService));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(QuitGroupCommand request, CancellationToken cancellationToken)
        {
            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var groupUser = await _groupUserRepository.GetGroupUserAsync(request.GroupId, myId);

            if (groupUser == null)
                throw new ClientException("操作失败", new List<string> { $"User {myId} is not in Group {request.GroupId}." });

            _groupUserRepository.Remove(groupUser);

            if (await _groupUserRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
            {
                // BackwardCompatibility: 为了兼容以前的聊天服务，需要向redis写入相关数据
                var group = await _groupRepository.GetGroupWithMembersAsync(request.GroupId);
                await UpdateRedisAsync(request, group, myId);

                return true;
            }

            throw new ApplicationException("操作失败");
        }

        #region BackwardCompatibility: 为了兼容以前的聊天服务，需要向redis写入相关数据
        private async Task UpdateRedisAsync(QuitGroupCommand request, Domain.AggregatesModel.GroupAggregate.Group group, Guid myId)
        {
            try
            {
                // 从redis去掉被删除的群成员
                await _chatServerRedisService.RemoveGroupMemberAsync(myId, group.ChatServerGroupId);

                // 发布系统消息
                var changedUsers = await _userRepository.GetUsersAsync(new List<Guid> { myId });
                await _chatServerRedisService.WriteGroupMemberMessageAsync(group, SysMsgType.MEMBER_SECEDED, changedUsers);
            }
            catch (Exception ex)
            {
                _logger.LogError("Redis Error: {@RedisError}", ex);
            }
        }
        #endregion
    }
}
