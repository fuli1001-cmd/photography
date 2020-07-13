using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.User.DisableUser
{
    /// <summary>
    /// 禁用或启用用户
    /// </summary>
    public class DisableUserCommand : IRequest<bool>
    {
        /// <summary>
        /// 禁用或启用的用户id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 禁用或启用
        /// </summary>
        public bool Disabled { get; set; }
    }
}
