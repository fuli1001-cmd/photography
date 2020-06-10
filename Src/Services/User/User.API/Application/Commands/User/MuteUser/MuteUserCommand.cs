using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.User.MuteUser
{
    public class MuteUserCommand : IRequest<bool>
    {
        /// <summary>
        /// 要免打扰的用户id
        /// </summary>
        [Required]
        public Guid UserId { get; set; }

        /// <summary>
        /// true：不接收消息（免打扰）
        /// false：接收消息
        /// </summary>
        [Required]
        public bool Muted { get; set; }
    }
}
