using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Notification.API.Application.Commands.CreatePost
{
    public class CreatePostCommand : IRequest<bool>
    {
        public Guid PostId { get; set; }

        public string Image { get; set; }
    }
}
