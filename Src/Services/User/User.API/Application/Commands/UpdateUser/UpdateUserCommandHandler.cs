using ApplicationMessages.Events;
using Arise.DDD.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.User.API.Infrastructure.Redis;
using Photography.Services.User.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.UpdateUser
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRedisService _redisService;
        private readonly ILogger<UpdateUserCommandHandler> _logger;
        private readonly IServiceProvider _serviceProvider;

        private IMessageSession _messageSession;

        public UpdateUserCommandHandler(IUserRepository userRepository,
            IServiceProvider serviceProvider, 
            IHttpContextAccessor httpContextAccessor,
            IRedisService redisService,
            ILogger<UpdateUserCommandHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _redisService = redisService ?? throw new ArgumentNullException(nameof(redisService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            // 检查是否是自己
            var userId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (userId != request.UserId)
                throw new DomainException("更新失败。");
            
            // 检查昵称是否已被别人占用
            var nicknameUser = await _userRepository.GetByNicknameAsync(request.Nickname);
            if (nicknameUser != null && nicknameUser.Id != userId)
                throw new DomainException("昵称已存在。");

            var user = await _userRepository.GetByIdAsync(request.UserId);
            user.Update(request.Nickname, request.Gender, request.Birthday, request.UserType, 
                request.Province, request.City, request.Sign, request.Avatar);

            _userRepository.Update(user);

            if (await _userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
            {
                await SendUserUpdatedEventAsync(request.UserId, request.Nickname, request.Avatar, request.UserType);
                return true;
            }
            else
                throw new DomainException("更新失败。");
        }

        private async Task SendUserUpdatedEventAsync(Guid userId, string nickName, string avatar, UserType? userType)
        {
            UserUpdatedEvent @event = new UserUpdatedEvent { UserId = userId, NickName = nickName, Avatar = avatar, UserType = (int?)userType };
            _messageSession = (IMessageSession)_serviceProvider.GetService(typeof(IMessageSession));
            await _messageSession.Publish(@event);
            _logger.LogInformation("----- Published UserUpdatedEvent: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);
        }
    }
}
