using MediatR;
using Photography.Services.Post.API.Application.Commands.Post.ToggleLikePost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.Post.ToggleUserLikePost
{
    /// <summary>
    /// 指定用户赞某个帖子
    /// </summary>
    public class ToggleUserLikePostCommand : ToggleLikePostCommand
    {
        /// <summary>
        /// 要点赞帖子的用户id数组
        /// </summary>
        public Guid UserId { get; set; }
    }
}
