using ApplicationMessages.Events;
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

namespace Photography.Services.Post.API.Application.Commands.Circle.AddCircleMember
{
    public class AddCircleMemberCommandHandler : IRequestHandler<AddCircleMemberCommand, bool>
    {
        private readonly ICircleRepository _circleRepository;
        private readonly IUserCircleRelationRepository _userCircleRelationRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AddCircleMemberCommandHandler> _logger;
        private readonly IServiceProvider _serviceProvider;

        private IMessageSession _messageSession;

        public AddCircleMemberCommandHandler(ICircleRepository circleRepository, 
            IUserCircleRelationRepository userCircleRelationRepository,
            IHttpContextAccessor httpContextAccessor,
            IServiceProvider serviceProvider,
            ILogger<AddCircleMemberCommandHandler> logger)
        {
            _circleRepository = circleRepository ?? throw new ArgumentNullException(nameof(circleRepository));
            _userCircleRelationRepository = userCircleRelationRepository ?? throw new ArgumentNullException(nameof(userCircleRelationRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(AddCircleMemberCommand request, CancellationToken cancellationToken)
        {
            var circle = await _circleRepository.GetByIdAsync(request.CircleId);

            if (circle == null)
                throw new ClientException("操作失败", new List<string> { $"Circle {request.CircleId} dos not exist." });

            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (circle.OwnerId != myId)
                throw new ClientException("当前用户不是圈主", new List<string> { $"Circle {request.CircleId} does not belong to user {myId}" });

            // 添加并发送用户已入圈事件
            if (await AddCircleMemberAsync(circle.Id, request.UserId, _userCircleRelationRepository, cancellationToken))
            {
                var messageSession = (IMessageSession)_serviceProvider.GetService(typeof(IMessageSession));
                await SendJoinedCircleEventAsync(circle, request.UserId, messageSession, _logger);
                return true;
            }

            throw new ApplicationException("操作失败");
        }

        /// <summary>
        /// 将用户加入圈子，使用静态方法是因为JoinCircleCommandHandler也需要加用户入圈
        /// </summary>
        /// <param name="circleId">圈子id</param>
        /// <param name="joinedUserId">要加入圈子的用户id</param>
        /// <param name="userCircleRelationRepository"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<bool> AddCircleMemberAsync(Guid circleId, Guid joinedUserId, 
            IUserCircleRelationRepository userCircleRelationRepository, CancellationToken cancellationToken)
        {
            var userCircle = await userCircleRelationRepository.GetRelationAsync(circleId, joinedUserId);

            if (userCircle == null)
            {
                userCircle = new UserCircleRelation(joinedUserId, circleId);

                userCircle.Join();
                userCircleRelationRepository.Add(userCircle);

                return await userCircleRelationRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
            }

            throw new ClientException("用户已在圈中", new List<string> { $"User {joinedUserId} is already in circle {circleId}" });
        }

        // 发送用户已入圈事件
        public static async Task SendJoinedCircleEventAsync(Domain.AggregatesModel.CircleAggregate.Circle circle, Guid joinedUserId, 
            IMessageSession messageSession, ILogger logger)
        {
            var @event = new JoinedCircleEvent
            {
                JoinedUserId = joinedUserId,
                CircleOwnerId = circle.OwnerId,
                CircleId = circle.Id,
                CircleName = circle.Name
            };

            await messageSession.Publish(@event);

            logger.LogInformation("----- Published JoinedCircleEvent: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);
        }
    }
}
