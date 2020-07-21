using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.Group.UpdateGroupAvatar
{
    /// <summary>
    /// 群内成员更改群头像
    /// </summary>
    public class UpdateGroupAvatarCommand : IRequest<bool>
    {
        /// <summary>
        /// 要更新群头像的群id
        /// </summary>
        public Guid GroupId { get; set; }

        /// <summary>
        /// 群头像
        /// </summary>
        public string Avatar { get; set; }
    }
}
