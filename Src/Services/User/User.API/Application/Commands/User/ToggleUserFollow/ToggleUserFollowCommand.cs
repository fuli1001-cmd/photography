using Photography.Services.User.API.Application.Commands.User.ToggleFollow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.User.ToggleUserFollow
{
    public class ToggleUserFollowCommand : ToggleFollowCommand
    {
        /// <summary>
        /// 粉丝id
        /// </summary>
        public Guid FollowerId { get; set; }
    }
}
