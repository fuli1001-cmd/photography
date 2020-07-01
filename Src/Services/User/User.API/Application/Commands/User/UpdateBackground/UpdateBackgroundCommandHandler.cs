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

namespace Photography.Services.User.API.Application.Commands.User.UpdateBackground
{
    public class UpdateBackgroundCommandHandler : IRequestHandler<UpdateBackgroundCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UpdateBackgroundCommandHandler> _logger;

        public UpdateBackgroundCommandHandler(IUserRepository userRepository,
            IServiceProvider serviceProvider,
            IHttpContextAccessor httpContextAccessor,
            ILogger<UpdateBackgroundCommandHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(UpdateBackgroundCommand request, CancellationToken cancellationToken)
        {
            Guid userId = Guid.Empty;

            // 历史原因：
            // 没有管理平台之前只有手机用户操作自己的数据，因此UpdateBackgroundCommand中没有要求传入UserId
            // 管理平台加入之后也是用这个API，但是管理平台必须传UserId，才能知道是操作的哪个用户的数据
            var role = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
            if (role == "admin")
            {
                if (request.UserId == null)
                    throw new ClientException("操作失败", new List<string> { $"UserId is need." });

                userId = request.UserId.Value;
            }
            else
            {
                userId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            }

            var user = await _userRepository.GetByIdAsync(userId);
            user.UpdateBackground(request.BackgroundImage);
            return await _userRepository.UnitOfWork.SaveEntitiesAsync();
        }
    }
}
