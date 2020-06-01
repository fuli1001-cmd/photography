using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.UpdateBackground
{
    public class UpdateBackgroundCommand : IRequest<bool>
    {
        public string BackgroundImage { get; set; }
    }
}
