using MediatR;
using Photography.Services.Order.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Order.API.Application.Commands.User.SetOrgAuthStatus
{
    /// <summary>
    /// 设置团体认证状态
    /// </summary>
    public class SetOrgAuthStatusCommand : IRequest<bool>
    {
        /// <summary>
        /// 要设置的用户id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 要设置成的认证状态
        /// </summary>
        public AuthStatus Status { get; set; }
    }
}
