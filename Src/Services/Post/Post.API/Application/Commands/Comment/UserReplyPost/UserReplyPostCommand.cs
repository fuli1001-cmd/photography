using Photography.Services.Post.API.Application.Commands.Comment.ReplyPost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.Comment.UserReplyPost
{
    public class UserReplyPostCommand : ReplyPostCommand
    {
        public Guid RepliedUserId { get; set; }
    }
}
