using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Order.API.Application.Commands.CreateUser
{
    public class CreateUserCommand : IRequest<bool>
    {
        public string Id { get; set; }
        public string UserName { get; set; }
    }
}
