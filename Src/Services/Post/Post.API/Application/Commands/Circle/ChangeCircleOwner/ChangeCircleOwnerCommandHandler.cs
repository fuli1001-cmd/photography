using ApplicationMessages.Events.Post;
using Arise.DDD.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.Post.Domain.AggregatesModel.CircleAggregate;
using Photography.Services.Post.Domain.AggregatesModel.UserCircleRelationAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.Circle.ChangeCircleOwner
{
    public class ChangeCircleOwnerCommandHandler : IRequestHandler<ChangeCircleOwnerCommand, bool>
    {
        private readonly ICircleRepository _circleRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ChangeCircleOwnerCommandHandler> _logger;
        private readonly IServiceProvider _serviceProvider;

        private IMessageSession _messageSession;

        public ChangeCircleOwnerCommandHandler(ICircleRepository circleRepository,
            IHttpContextAccessor httpContextAccessor,
            IServiceProvider serviceProvider,
            ILogger<ChangeCircleOwnerCommandHandler> logger)
        {
            _circleRepository = circleRepository ?? throw new ArgumentNullException(nameof(circleRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ChangeCircleOwnerCommand request, CancellationToken cancellationToken)
        {
            var circle = await _circleRepository.GetCircleWithUsersAsync(request.CircleId);

            if (circle == null)
                throw new ClientException("操作失败", new List<string> { $"Circle {request.CircleId} dos not exist." });

            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            circle.SetOwner(myId, request.UserId);

            if (await _circleRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
            {
                await SendCircleOwnerChangedEventAsync(circle, myId);
                return true;
            }

            throw new ApplicationException("操作失败");
        }

        private async Task SendCircleOwnerChangedEventAsync(Domain.AggregatesModel.CircleAggregate.Circle circle, Guid operatorId)
        {
            var @event = new CircleOwnerChangedEvent
            {
                OldOwnerId = operatorId,
                NewOwnerId = circle.OwnerId,
                CircleId = circle.Id,
                CircleName = circle.Name
            };

            var messageSession = (IMessageSession)_serviceProvider.GetService(typeof(IMessageSession));
            await messageSession.Publish(@event);

            _logger.LogInformation("----- Published CircleOwnerChangedEvent: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);
        }
    }
}
