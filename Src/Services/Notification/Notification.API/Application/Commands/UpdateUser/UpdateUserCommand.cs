using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Notification.API.Application.Commands.UpdateUser
{
    public class UpdateUserCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }

        public string NickName { get; set; }

        public string Avatar { get; set; }
    }
}
