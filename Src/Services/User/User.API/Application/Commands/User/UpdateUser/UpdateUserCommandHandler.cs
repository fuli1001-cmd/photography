using ApplicationMessages.Events;
using Arise.DDD.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.User.API.BackwardCompatibility.ChatServerRedis;
using Photography.Services.User.API.Infrastructure.Redis;
using Photography.Services.User.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.User.UpdateUser
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IChatServerRedis _chatServerRedisService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UpdateUserCommandHandler> _logger;
        private readonly IServiceProvider _serviceProvider;

        private IMessageSession _messageSession;

        public UpdateUserCommandHandler(IUserRepository userRepository,
            IServiceProvider serviceProvider, 
            IHttpContextAccessor httpContextAccessor,
            IChatServerRedis chatServerRedisService,
            ILogger<UpdateUserCommandHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _chatServerRedisService = chatServerRedisService ?? throw new ArgumentNullException(nameof(chatServerRedisService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            // 检查是否是自己
            var userId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (userId != request.UserId)
                throw new ClientException("操作失败", new List<string> { $"Current user is not {request.UserId}." });
            
            // 检查昵称是否已被别人占用
            var nicknameUser = await _userRepository.GetByNicknameAsync(request.Nickname);
            if (nicknameUser != null && nicknameUser.Id != userId)
                throw new ClientException("昵称已存在");

            var user = await _userRepository.GetByIdAsync(request.UserId);
            user.Update(request.Nickname, request.Gender, request.Birthday, request.UserType, 
                request.Province, request.City, request.Sign, request.Avatar);

            _userRepository.Update(user);

            if (await _userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
            {
                await SendUserUpdatedEventAsync(request.UserId, request.Nickname, request.Avatar, request.UserType);
                await UpdateRedisAsync(user);
                return true;
            }
            
            throw new ApplicationException("操作失败");
        }

        private async Task SendUserUpdatedEventAsync(Guid userId, string nickName, string avatar, UserType? userType)
        {
            UserUpdatedEvent @event = new UserUpdatedEvent { UserId = userId, NickName = nickName, Avatar = avatar, UserType = (int?)userType };
            _messageSession = (IMessageSession)_serviceProvider.GetService(typeof(IMessageSession));
            await _messageSession.Publish(@event);
            _logger.LogInformation("----- Published UserUpdatedEvent: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);
        }

        #region BackwardCompatibility: 为了兼容以前的聊天服务，需要向redis写入相关数据
        private async Task UpdateRedisAsync(Domain.AggregatesModel.UserAggregate.User user)
        {
            try
            {
                // 向redis中写入用户信息，供聊天服务使用
                await _chatServerRedisService.WriteUserAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError("Redis Error: {@RedisError}", ex);
            }
        }
        #endregion
    }
}
