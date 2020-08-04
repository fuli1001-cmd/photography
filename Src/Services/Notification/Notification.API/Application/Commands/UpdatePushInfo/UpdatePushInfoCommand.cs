using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Notification.API.Application.Commands.UpdatePushInfo
{
    public class UpdatePushInfoCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }

        public string RegistrationId { get; set; }
    }
}
