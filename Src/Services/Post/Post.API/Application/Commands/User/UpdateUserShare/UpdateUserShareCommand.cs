using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.User.UpdateUserShare
{
    public class UpdateUserShareCommand : IRequest<bool>
    {
        /// <summary>
        /// 分享者id
        /// </summary>
        public Guid SharerId { get; set; }

        /// <summary>
        /// 分享的帖子id
        /// </summary>
        public Guid? PostId { get; set; }

        /// <summary>
        /// 分享的帖子类别
        /// </summary>
        public string PrivateTag { get; set; }
    }
}
