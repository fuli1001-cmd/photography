﻿using ApplicationMessages.Events;
using Arise.DDD.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.Post.API.Application.Commands.Circle.AddCircleMember;
using Photography.Services.Post.Domain.AggregatesModel.CircleAggregate;
using Photography.Services.Post.Domain.AggregatesModel.UserCircleRelationAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.Circle.JoinCircle
{
    public class JoinCircleCommandHandler : IRequestHandler<JoinCircleCommand, bool>
    {
        private readonly ICircleRepository _circleRepository;
        private readonly IUserCircleRelationRepository _userCircleRelationRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<JoinCircleCommandHandler> _logger;
        private readonly IServiceProvider _serviceProvider;

        private IMessageSession _messageSession;

        public JoinCircleCommandHandler(ICircleRepository circleRepository,
            IUserCircleRelationRepository userCircleRelationRepository,
            IHttpContextAccessor httpContextAccessor,
            IServiceProvider serviceProvider,
            ILogger<JoinCircleCommandHandler> logger)
        {
            _circleRepository = circleRepository ?? throw new ArgumentNullException(nameof(circleRepository));
            _userCircleRelationRepository = userCircleRelationRepository ?? throw new ArgumentNullException(nameof(userCircleRelationRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(JoinCircleCommand request, CancellationToken cancellationToken)
        {
            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userCircle = await _userCircleRelationRepository.GetRelationAsync(request.CircleId, myId);

            if (userCircle == null)
            {
                var circle = await _circleRepository.GetByIdAsync(request.CircleId);

                if (circle == null)
                    throw new ClientException("操作失败", new List<string> { $"Circle {request.CircleId} dos not exist." });

                _messageSession = (IMessageSession)_serviceProvider.GetService(typeof(IMessageSession));

                if (circle.VerifyJoin)
                {
                    // 发送用户申请入圈事件
                    await SendAppliedJoinCircleEventAsync(myId, circle, request.Description);
                    return true;
                }

                // 无需审核，直接入圈，创建user circle关系
                ////await _addCircleMemberCommandHandler.AddCircleMemberAsync(circle, myId, cancellationToken);
                //userCircle = new UserCircleRelation(myId, request.CircleId);
                //_userCircleRelationRepository.Add(userCircle);
                //if (await _userCircleRelationRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
                //{
                //    await SendJoinedCircleEventAsync(myId, circle);
                //    return true;
                //}
                //添加并发送用户已入圈事件
                if (await AddCircleMemberCommandHandler.AddCircleMemberAsync(circle.Id, myId, _userCircleRelationRepository, cancellationToken))
                {
                    await AddCircleMemberCommandHandler.SendJoinedCircleEventAsync(circle, myId, _messageSession, _logger);
                    return true;
                }

                // 保存失败
                throw new ApplicationException("操作失败");
            }

            throw new ClientException("操作失败", new List<string> { $"User {myId} is already in circle {request.CircleId}" });
        }

        // 发送用户申请入圈事件
        private async Task SendAppliedJoinCircleEventAsync(Guid myId, Domain.AggregatesModel.CircleAggregate.Circle circle, string applyDescription)
        {
            var @event = new AppliedJoinCircleEvent
            {
                ApplyUserId = myId,
                CircleOwnerId = circle.OwnerId,
                CircleId = circle.Id,
                CircleName = circle.Name,
                ApplyDescription = applyDescription
            };

            await _messageSession.Publish(@event);

            _logger.LogInformation("----- Published AppliedJoinCircleEvent: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);
        }
    }
}
