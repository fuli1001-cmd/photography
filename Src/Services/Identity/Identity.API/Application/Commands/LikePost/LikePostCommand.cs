using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Photography.Services.Identity.API.Application.Commands.LikePost
{
    [DataContract]
    public class LikePostCommand : IRequest<bool>
    {
        /// <summary>
        /// 点赞的帖子id
        /// </summary>
        [DataMember]
        [Required]
        public Guid PostId { get; set; }
    }
}
