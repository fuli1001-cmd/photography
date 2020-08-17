using ApplicationMessages.Events.User;
using Arise.DDD.Domain.Exceptions;
using Arise.DDD.Infrastructure.Redis;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.User.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.User.AuthOrg
{
    public class AuthOrgCommandHandler : IRequestHandler<AuthOrgCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRedisService _redisService;
        private readonly ILogger<AuthOrgCommandHandler> _logger;
        private readonly IServiceProvider _serviceProvider;

        private IMessageSession _messageSession;

        public AuthOrgCommandHandler(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor,
            IServiceProvider serviceProvider, IRedisService redisService, ILogger<AuthOrgCommandHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _redisService = redisService ?? throw new ArgumentNullException(nameof(redisService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(AuthOrgCommand request, CancellationToken cancellationToken)
        {
            // verify code
            var storedCode = await _redisService.StringGetAsync(request.OrgOperatorPhoneNumber);
            if (string.IsNullOrEmpty(storedCode) || storedCode.ToLower() != request.Code.ToLower())
                throw new ClientException("验证码错误");

            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = await _userRepository.GetByIdAsync(myId);

            user.SetOrgAuthInfo(request.OrgType, request.OrgSchoolName, request.OrgName, request.OrgDesc, 
                request.OrgOperatorName, request.OrgOperatorPhoneNumber, request.OrgImage);

            if (await _userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
            {
                // 保存成功，发送用户团体认证状态改变事件
                // 此处不等待，在后台发送事件即可
                SendUserOrgAuthStatusChangedEventAsync(myId, IdAuthStatus.Authenticating);

                // 保存成功，删除redis数据
                // 此处不等待，在后台删除redis数据即可
                _redisService.KeyDeleteAsync(request.OrgOperatorPhoneNumber);

                return true;
            }

            throw new ApplicationException("操作失败");
        }

        private async Task SendUserOrgAuthStatusChangedEventAsync(Guid userId, IdAuthStatus status)
        {
            var @event = new UserOrgAuthStatusChangedEvent { UserId = userId, Status = (int)status };
            _messageSession = (IMessageSession)_serviceProvider.GetService(typeof(IMessageSession));
            await _messageSession.Publish(@event);
            _logger.LogInformation("----- Published UserOrgAuthStatusChangedEvent: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);
        }
    }
}
