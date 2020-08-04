using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.Circle.ChangeCircleOwner
{
    /// <summary>
    /// 设置圈主的命令
    /// </summary>
    public class ChangeCircleOwnerCommand : IRequest<bool>
    {
        /// <summary>
        /// 圈子ID
        /// </summary>
        public Guid CircleId { get; set; }

        /// <summary>
        /// 要设置为圈主的用户id
        /// </summary>
        public Guid UserId { get; set; }
    }
}
