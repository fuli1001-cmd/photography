using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.User.UpdateBackground
{
    public class UpdateBackgroundCommand : IRequest<bool>
    {
        public Guid? UserId { get; set; }
        public string BackgroundImage { get; set; }
    }
}
