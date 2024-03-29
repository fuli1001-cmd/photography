﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.Circle.AddCircleMember
{
    /// <summary>
    /// 圈主将用户加入圈子的命令
    /// </summary>
    public class AddCircleMemberCommand : IRequest<bool>
    {
        /// <summary>
        /// 圈子ID
        /// </summary>
        public Guid CircleId { get; set; }

        /// <summary>
        /// 要加入圈子的用户id
        /// </summary>
        public Guid UserId { get; set; }
    }
}
