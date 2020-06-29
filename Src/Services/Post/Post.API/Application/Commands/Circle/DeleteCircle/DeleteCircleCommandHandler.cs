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

namespace Photography.Services.Post.API.Application.Commands.Circle.DeleteCircle
{
    public class DeleteCircleCommandHandler : IRequestHandler<DeleteCircleCommand, bool>
    {
        private readonly ICircleRepository _circleRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<DeleteCircleCommandHandler> _logger;

        public DeleteCircleCommandHandler(ICircleRepository circleRepository,
            IHttpContextAccessor httpContextAccessor,
            ILogger<DeleteCircleCommandHandler> logger)
        {
            _circleRepository = circleRepository ?? throw new ArgumentNullException(nameof(circleRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(DeleteCircleCommand request, CancellationToken cancellationToken)
        {
            var circle = await _circleRepository.GetByIdAsync(request.Id);
            if (circle == null)
                throw new ClientException("圈子不存在");

            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            circle.Delete(myId);
            _circleRepository.Remove(circle);

            return await _circleRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
