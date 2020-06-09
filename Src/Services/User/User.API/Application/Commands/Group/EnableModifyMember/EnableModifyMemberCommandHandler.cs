using Arise.DDD.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Photography.Services.User.API.BackwardCompatibility.ChatServerRedis;
using Photography.Services.User.Domain.AggregatesModel.GroupAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.Group.EnableModifyMember
{
    public class EnableModifyMemberCommandHandler : IRequestHandler<EnableModifyMemberCommand, bool>
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IChatServerRedis _chatServerRedisService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<EnableModifyMemberCommandHandler> _logger;

        public EnableModifyMemberCommandHandler(IGroupRepository groupRepository,
            IChatServerRedis chatServerRedisService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<EnableModifyMemberCommandHandler> logger)
        {
            _groupRepository = groupRepository ?? throw new ArgumentNullException(nameof(groupRepository));
            _chatServerRedisService = chatServerRedisService ?? throw new ArgumentNullException(nameof(chatServerRedisService));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(EnableModifyMemberCommand request, CancellationToken cancellationToken)
        {
            var group = await _groupRepository.GetGroupWithMembersAsync(request.GroupId);
            if (group == null)
            {
                _logger.LogError("MuteGroupCommandHandler: Group {GroupId} does not exist.", request.GroupId);
                throw new DomainException("操作失败。");
            }

            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (request.Enabled)
                group.EnableAddMember(myId);
            else
                group.DisableAddMember(myId);

            if (await _groupRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
            {
                // BackwardCompatibility: 为了兼容以前的聊天服务，需要向redis写入相关数据
                await _chatServerRedisService.WriteGroupAsync(group);

                return true;
            }

            throw new DomainException("操作失败。");
        }
    }
}
