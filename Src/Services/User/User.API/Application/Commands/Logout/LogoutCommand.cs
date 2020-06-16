using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.Logout
{
    public class LogoutCommand : IRequest<bool>
    {
    }
}
