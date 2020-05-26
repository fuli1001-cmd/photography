using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.Comment.ToggleLikeComment
{
    [DataContract]
    public class ToggleLikeCommentCommand : IRequest<bool>
    {
        /// <summary>
        /// 要赞的评论id
        /// </summary>
        [DataMember]
        [Required]
        public Guid CommentId { get; set; }
    }
}
