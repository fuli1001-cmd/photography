using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.Post.ReplyPost
{
    [DataContract]
    public class ReplyPostCommand : IRequest<bool>
    {
        /// <summary>
        /// 评论内容
        /// </summary>
        [DataMember]
        [Required]
        public string Text { get; set; }

        /// <summary>
        /// 要评论的帖子id
        /// </summary>
        [DataMember]
        [Required]
        public Guid PostId { get; set; }
    }
}
