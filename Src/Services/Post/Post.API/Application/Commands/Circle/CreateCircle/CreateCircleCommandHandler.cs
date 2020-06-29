using Arise.DDD.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.API.Query.Interfaces;
using Photography.Services.Post.API.Query.ViewModels;
using Photography.Services.Post.Domain.AggregatesModel.CircleAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.Circle.CreateCircle
{
    public class CreateCircleCommandHandler : IRequestHandler<CreateCircleCommand, CircleViewModel>
    {
        private readonly ICircleRepository _circleRepository;
        private readonly ICircleQueries _circleQueries;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CreateCircleCommandHandler> _logger;

        public CreateCircleCommandHandler(ICircleRepository circleRepository,
            ICircleQueries circleQueries,
            IHttpContextAccessor httpContextAccessor,
            ILogger<CreateCircleCommandHandler> logger)
        {
            _circleRepository = circleRepository ?? throw new ArgumentNullException(nameof(circleRepository));
            _circleQueries = circleQueries ?? throw new ArgumentNullException(nameof(circleQueries));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<CircleViewModel> Handle(CreateCircleCommand request, CancellationToken cancellationToken)
        {
            var circle = await _circleRepository.GetCircleByNameAsync(request.Name);
            if (circle != null)
                throw new ClientException("圈子名已存在");

            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            circle = new Domain.AggregatesModel.CircleAggregate.Circle(request.Name, request.Description, request.VerifyJoin, request.BackgroundImage, myId);
            _circleRepository.Add(circle);

            if (await _circleRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
                return await _circleQueries.GetCircleAsync(circle.Id);

            throw new ApplicationException("操作失败");
        }
    }
}
