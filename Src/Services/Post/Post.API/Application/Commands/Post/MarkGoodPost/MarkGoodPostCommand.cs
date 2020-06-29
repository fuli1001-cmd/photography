using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.Post.MarkGoodPost
{
    public class MarkGoodPostCommand : IRequest<bool>
    {
        /// <summary>
        /// 帖子id
        /// </summary>
        public Guid PostId { get; set; }

        /// <summary>
        /// 是否设为精华
        /// </summary>
        public bool Good { get; set; }
    }
}
