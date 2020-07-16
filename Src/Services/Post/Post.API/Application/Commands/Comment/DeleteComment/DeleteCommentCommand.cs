using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.Comment.DeleteComment
{
    public class DeleteCommentCommand : IRequest<bool>
    {
        /// <summary>
        /// 要删除的评论id
        /// </summary>
        [Required]
        public Guid CommentId { get; set; }
    }
}
