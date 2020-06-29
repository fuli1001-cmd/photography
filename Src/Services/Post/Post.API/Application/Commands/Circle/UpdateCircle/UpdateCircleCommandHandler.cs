using Arise.DDD.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.Domain.AggregatesModel.CircleAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.Circle.UpdateCircle
{
    public class UpdateCircleCommandHandler : IRequestHandler<UpdateCircleCommand, bool>
    {
        private readonly ICircleRepository _circleRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UpdateCircleCommandHandler> _logger;

        public UpdateCircleCommandHandler(ICircleRepository circleRepository,
            IHttpContextAccessor httpContextAccessor,
            ILogger<UpdateCircleCommandHandler> logger)
        {
            _circleRepository = circleRepository ?? throw new ArgumentNullException(nameof(circleRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(UpdateCircleCommand request, CancellationToken cancellationToken)
        {
            var circle = await _circleRepository.GetByIdAsync(request.Id);
            
            if (circle == null)
                throw new ClientException("圈子不存在");

            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            circle.Update(request.Name, request.Description, request.VerifyJoin, request.BackgroundImage, myId);

            return await _circleRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
