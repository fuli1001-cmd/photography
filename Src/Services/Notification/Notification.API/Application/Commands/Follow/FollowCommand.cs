using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Notification.API.Application.Commands.Follow
{
    public class FollowCommand : IRequest<bool>
    {
        public Guid FollowerId { get; set; }

        public Guid FollowedUserId { get; set; }
    }
}
