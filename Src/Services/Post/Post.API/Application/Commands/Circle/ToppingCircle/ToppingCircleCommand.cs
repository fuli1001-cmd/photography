﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.Circle.ToppingCircle
{
    public class ToppingCircleCommand : IRequest<bool>
    {
        /// <summary>
        /// 圈子ID
        /// </summary>
        public Guid CircleId { get; set; }
    }
}
