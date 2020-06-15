using Arise.DDD.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Photography.Services.User.API.BackwardCompatibility.ChatServerRedis;
using Photography.Services.User.Domain.AggregatesModel.GroupAggregate;
using Photography.Services.User.Domain.AggregatesModel.GroupUserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.Group.MuteGroup
{
    public class MuteGroupCommandHandler : IRequestHandler<MuteGroupCommand, bool>
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IGroupUserRepository _groupUserRepository;
        private readonly IChatServerRedis _chatServerRedisService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<MuteGroupCommandHandler> _logger;

        public MuteGroupCommandHandler(
            IGroupRepository groupRepository,
            IGroupUserRepository groupUserRepository,
            IChatServerRedis chatServerRedisService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<MuteGroupCommandHandler> logger)
        {
            _groupRepository = groupRepository ?? throw new ArgumentNullException(nameof(groupRepository));
            _groupUserRepository = groupUserRepository ?? throw new ArgumentNullException(nameof(groupUserRepository));
            _chatServerRedisService = chatServerRedisService ?? throw new ArgumentNullException(nameof(chatServerRedisService));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(MuteGroupCommand request, CancellationToken cancellationToken)
        {
            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var groupUser = await _groupUserRepository.GetGroupUserAsync(request.GroupId, myId);
            if (groupUser == null)
                throw new ClientException("操作失败。", new List<string> { $"User {myId} is not in Group {request.GroupId}." });

            if (request.Muted)
                groupUser.Mute();
            else
                groupUser.UnMute();

            if (await _groupRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
            {
                // BackwardCompatibility: 为了兼容以前的聊天服务，需要向redis写入相关数据
                var group = await _groupRepository.GetByIdAsync(request.GroupId);
                await UpdateRedisAsync(group, groupUser);

                return true;
            }

            throw new ApplicationException("操作失败。");
        }

        #region BackwardCompatibility: 为了兼容以前的聊天服务，需要向redis写入相关数据
        private async Task UpdateRedisAsync(Domain.AggregatesModel.GroupAggregate.Group group, GroupUser groupUser)
        {
            try
            {
                // 向redis写入群成员
                await _chatServerRedisService.WriteGroupMemberAsync(groupUser.UserId.Value, group.ChatServerGroupId, groupUser.Muted ? 1 : 0);
            }
            catch (Exception ex)
            {
                _logger.LogError("Redis Error: {@RedisError}", ex);
            }
        }
        #endregion
    }
}
