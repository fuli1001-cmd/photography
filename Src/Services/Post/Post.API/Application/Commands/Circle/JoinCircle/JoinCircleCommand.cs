using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.Circle.JoinCircle
{
    /// <summary>
    /// 申请加圈命令
    /// </summary>
    public class JoinCircleCommand : IRequest<bool>
    {
        /// <summary>
        /// 圈子ID
        /// </summary>
        public Guid CircleId { get; set; }

        /// <summary>
        /// 申请描述
        /// </summary>
        public string Description { get; set; }
    }
}
