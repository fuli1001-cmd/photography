using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.Post.MovePostOutFromCircle
{
    public class MovePostOutFromCircleCommand : IRequest<bool>
    {
        /// <summary>
        /// 帖子id
        /// </summary>
        public Guid PostId { get; set; }
    }
}
