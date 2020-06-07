using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.Group.CreateGroup
{
    public class CreateGroupCommand : IRequest<bool>
    {
        public string Name { get; set; }
        public string Avatar { get; set; }
        public List<Guid> UserIds { get; set; }
    }
}
