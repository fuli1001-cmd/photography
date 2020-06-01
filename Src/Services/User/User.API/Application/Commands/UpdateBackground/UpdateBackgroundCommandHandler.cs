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

namespace Photography.Services.User.API.Application.Commands.UpdateBackground
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
            var userId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var user = await _userRepository.GetByIdAsync(userId);
            user.UpdateBackground(request.BackgroundImage);
            return await _userRepository.UnitOfWork.SaveEntitiesAsync();
        }
    }
}
