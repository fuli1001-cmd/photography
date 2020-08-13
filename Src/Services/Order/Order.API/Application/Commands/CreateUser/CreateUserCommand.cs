using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Order.API.Application.Commands.CreateUser
{
    public class CreateUserCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }

        public string NickName { get; set; }

        public int ChatServerUserId { get; set; }
    }
}
