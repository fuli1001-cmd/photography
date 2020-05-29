using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.User.CreateUser
{
    public class CreateUserCommand : IRequest<bool>
    {
        public string UserId { get; set; }

        public string UserName { get; set; }
    }
}
