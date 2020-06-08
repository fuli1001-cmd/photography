﻿using Arise.DDD.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Photography.Services.User.Domain.AggregatesModel.GroupAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.Group.DeleteGroup
{
    public class DeleteGroupCommandHandler : IRequestHandler<DeleteGroupCommand, bool>
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<DeleteGroupCommandHandler> _logger;

        public DeleteGroupCommandHandler(IGroupRepository groupRepository,
            IHttpContextAccessor httpContextAccessor,
            ILogger<DeleteGroupCommandHandler> logger)
        {
            _groupRepository = groupRepository ?? throw new ArgumentNullException(nameof(groupRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
        {
            var group = await _groupRepository.GetByIdAsync(request.GroupId);
            if (group == null)
            {
                _logger.LogError("DismissGroupCommandHandler: Group {GroupId} does not exist.", request.GroupId);
                throw new DomainException("操作失败。");
            }

            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (myId != group.OwnerId)
            {
                _logger.LogError("DismissGroupCommandHandler: Group {GroupId} does not belong to user {UserId}.", request.GroupId, myId);
                throw new DomainException("操作失败。");
            }

            group.Delete();

            _groupRepository.Remove(group);

            return await _groupRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
