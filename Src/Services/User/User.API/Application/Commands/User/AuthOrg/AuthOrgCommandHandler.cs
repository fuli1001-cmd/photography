using Arise.DDD.Domain.Exceptions;
using Arise.DDD.Infrastructure.Redis;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
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

        public AuthOrgCommandHandler(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor, IRedisService redisService, ILogger<AuthOrgCommandHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
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
                // 保存成功，删除redis数据
                // 此处不等待，在后台删除redis数据即可
                _redisService.KeyDeleteAsync(request.OrgOperatorPhoneNumber);
                return true;
            }

            throw new ApplicationException("操作失败");
        }
    }
}
