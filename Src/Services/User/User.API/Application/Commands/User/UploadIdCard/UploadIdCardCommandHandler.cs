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

namespace Photography.Services.User.API.Application.Commands.User.UploadIdCard
{
    public class UploadIdCardCommandHandler : IRequestHandler<UploadIdCardCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UploadIdCardCommandHandler> _logger;
        
        public UploadIdCardCommandHandler(IUserRepository userRepository,
            IHttpContextAccessor httpContextAccessor,
            ILogger<UploadIdCardCommandHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(UploadIdCardCommand request, CancellationToken cancellationToken)
        {
            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = await _userRepository.GetByIdAsync(myId);

            if (user == null)
                throw new ClientException("操作失败", new List<string> { $"User {myId} does not exist." });

            user.SetIdCard(request.IdCardFront, request.IdCardBack, request.IdCardHold);

            return await _userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
