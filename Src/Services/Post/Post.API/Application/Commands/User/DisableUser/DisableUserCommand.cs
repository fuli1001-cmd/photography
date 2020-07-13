using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.User.DisableUser
{
    /// <summary>
    /// 禁用用户
    /// </summary>
    public class DisableUserCommand : IRequest<bool>
    {
        /// <summary>
        /// 被禁用的用户id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 被禁用到的时间点
        /// </summary>
        public DateTime? DisabledTime { get; set; }
    }
}
