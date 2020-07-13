using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.User.AuthRealName
{
    /// <summary>
    /// 实名认证
    /// </summary>
    public class AuthRealNameCommand : IRequest<bool>
    {
        /// <summary>
        /// 被审核用户id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 是否通过审核
        /// </summary>
        public bool Passed { get; set; }
    }
}
