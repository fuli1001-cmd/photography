using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.Post.Share
{
    /// <summary>
    /// 分享命令
    /// </summary>
    public class ShareCommand : IRequest<bool>
    {
        /// <summary>
        /// 分享的帖子id
        /// </summary>
        public List<Guid> PostIds { get; set; }
    }
}
