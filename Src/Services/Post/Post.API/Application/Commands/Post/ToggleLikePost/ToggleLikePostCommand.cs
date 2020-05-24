using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.Post.ToggleLikePost
{
    [DataContract]
    public class ToggleLikePostCommand : IRequest<bool>
    {
        /// <summary>
        /// 点赞的帖子id
        /// </summary>
        [DataMember]
        [Required]
        public Guid PostId { get; set; }
    }
}
