using Arise.DDD.Domain.Exceptions;
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

namespace Photography.Services.User.API.Application.Commands.UpdateUser
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UpdateUserCommandHandler> _logger;

        public UpdateUserCommandHandler(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor, ILogger<UpdateUserCommandHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
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
            _logger.LogInformation("****************{@user}******************", user);
            user.Update(request.Nickname, request.Gender, request.Birthday, request.UserType, request.Province, request.City, request.Sign);

            _userRepository.Update(user);

            return await _userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
