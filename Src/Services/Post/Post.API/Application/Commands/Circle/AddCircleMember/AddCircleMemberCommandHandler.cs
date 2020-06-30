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
                throw new ClientException("操作失败", new List<string> { $"Circle {request.CircleId} does not belong to user {myId}" });

            return await AddCircleMemberAsync(circle, request.UserId, cancellationToken);
        }

        /// <summary>
        /// 将用户家圈子
        /// </summary>
        /// <param name="circle">圈子对象</param>
        /// <param name="joinedUserId">要加入圈子的用户id</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> AddCircleMemberAsync(Domain.AggregatesModel.CircleAggregate.Circle circle, Guid joinedUserId, CancellationToken cancellationToken)
        {
            var userCircle = await _userCircleRelationRepository.GetRelationAsync(circle.Id, joinedUserId);

            // 如果用户不在圈子则加入圈子，如果已在，直接返回true
            if (userCircle == null)
            {
                userCircle = new UserCircleRelation(joinedUserId, circle.Id);
                
                _userCircleRelationRepository.Add(userCircle);

                if (await _userCircleRelationRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
                {
                    // 发送用户已加入圈子的事件
                    await SendJoinedCircleEventAsync(joinedUserId, circle);
                    return true;
                }

                // 保存失败
                throw new ApplicationException("操作失败");
            }

            return true;
        }

        // 发送用户申请入圈事件
        private async Task SendJoinedCircleEventAsync(Guid joinedUserId, Domain.AggregatesModel.CircleAggregate.Circle circle)
        {
            var @event = new JoinedCircleEvent
            {
                JoinedUserId = joinedUserId,
                CircleOwnerId = circle.OwnerId,
                CircleName = circle.Name
            };

            _messageSession = (IMessageSession)_serviceProvider.GetService(typeof(IMessageSession));
            await _messageSession.Publish(@event);

            _logger.LogInformation("----- Published JoinedCircleEvent: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);
        }
    }
}
