using MediatR;
using Microsoft.Extensions.Logging;
using Photography.Services.User.Domain.AggregatesModel.GroupAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.Group.CreateGroup
{
    public class CreateGroupCommandHandler : IRequestHandler<CreateGroupCommand, bool>
    {
        private readonly IGroupRepository _groupRepository;
        private readonly ILogger<CreateGroupCommandHandler> _logger;

        public CreateGroupCommandHandler(IGroupRepository groupRepository, ILogger<CreateGroupCommandHandler> logger)
        {
            _groupRepository = groupRepository ?? throw new ArgumentNullException(nameof(groupRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<bool> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
