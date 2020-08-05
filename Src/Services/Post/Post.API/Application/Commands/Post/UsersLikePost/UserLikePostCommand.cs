using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.Post.UsersLikePost
{
    /// <summary>
    /// 指定用户赞某个帖子
    /// </summary>
    public class UserLikePostCommand : IRequest<bool>
    {
        /// <summary>
        /// 要点赞帖子的用户id数组
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 要被赞的帖子id
        /// </summary>
        public Guid PostId { get; set; }
    }
}
