using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.CreateUser
{
    public class CreateUserCommand : IRequest<bool>
    {
        public string Id { get; set; }
        
        public string UserName { get; set; }

        public string PhoneNumber { get; set; }

        public string Code { get; set; }
    }
}
