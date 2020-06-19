using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.User.AllowViewFollowedUsers
{
    public class AllowViewFollowedUsersCommand : IRequest<bool>
    {
        public bool Value { get; set; }
    }
}
