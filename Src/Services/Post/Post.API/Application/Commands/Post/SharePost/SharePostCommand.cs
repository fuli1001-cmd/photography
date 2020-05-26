﻿using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.Post.SharePost
{
    public class SharePostCommand : IRequest<int>
    {
        /// <summary>
        /// 点分享的帖子id
        /// </summary>
        [Required]
        public Guid PostId { get; set; }
    }
}