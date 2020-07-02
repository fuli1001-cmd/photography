using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.Post.SharePost
{
    public class SharePostCommand : IRequest<bool>
    {
        /// <summary>
        /// 点分享的帖子id
        /// </summary>
        public List<Guid> PostIds { get; set; }

        public List<Guid> PrivateTagIds { get; set; }

        public bool UnSpecifiedPrivateTag { get; set; }
    }
}
