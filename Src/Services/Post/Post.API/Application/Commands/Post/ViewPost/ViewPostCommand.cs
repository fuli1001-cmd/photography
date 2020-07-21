using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.Post.ViewPost
{
    /// <summary>
    /// 浏览帖子
    /// </summary>
    public class ViewPostCommand : IRequest<bool>
    {
        public Guid PostId { get; set; }
    }
}
