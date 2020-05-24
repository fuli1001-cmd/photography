using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.Post.ReplyComment
{
    [DataContract]
    public class ReplyCommentCommand : IRequest<bool>
    {
        /// <summary>
        /// 评论内容
        /// </summary>
        [DataMember]
        [Required]
        public string Text { get; set; }

        /// <summary>
        /// 要评论的评论id
        /// </summary>
        [DataMember]
        [Required]
        public Guid CommentId { get; set; }
    }
}
