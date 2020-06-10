using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.User.FollowEachOther
{
    public class FollowEachOtherCommand : IRequest<bool>
    {
        public string UserId { get; set; }

        public string InvitingUserCode { get; set; }
    }
}
