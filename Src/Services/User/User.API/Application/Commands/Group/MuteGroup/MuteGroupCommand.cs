using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.Group.MuteGroup
{
    public class MuteGroupCommand : IRequest<bool>
    {
        /// <summary>
        /// 要免打扰的群id
        /// </summary>
        [Required]
        public Guid GroupId { get; set; }

        /// <summary>
        /// true：不接收消息（免打扰）
        /// false：接收消息
        /// </summary>
        [Required]
        public bool Muted { get; set; }
    }
}
