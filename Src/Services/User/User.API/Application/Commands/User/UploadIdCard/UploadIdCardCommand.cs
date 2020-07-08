using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.User.UploadIdCard
{
    public class UploadIdCardCommand : IRequest<bool>
    {
        public string IdCardFront { get; set; }

        public string IdCardBack { get; set; }

        public string IdCardHold { get; set; }
    }
}
