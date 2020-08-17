using MediatR;
using Photography.Services.User.API.Query.ViewModels;
using Photography.Services.User.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.User.SetOrgAuthStatus
{
    /// <summary>
    /// 设置团体认证状态
    /// </summary>
    public class SetOrgAuthStatusCommand : IRequest<UserOrgAuthInfo>
    {
        /// <summary>
        /// 要设置的用户id
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// 要设置成的认证状态
        /// </summary>
        public IdAuthStatus Status { get; set; }
    }
}
