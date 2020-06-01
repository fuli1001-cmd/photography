using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.API.Query.ViewModels
{
    public class CommentViewModel
    {
        public Guid Id { get; set; }

        public string Text { get; set; }

        // 赞该条评论的数量
        public int Likes { get; set; }

        // 当前用户是否已赞该评论
        public bool Liked { get; set; }

        public double CreatedTime { get; set; }

        // 该条评论的子评论总数量
        public int SubCommentsCount { get; set; }

        // 该条评论的子评论
        public IEnumerable<CommentViewModel> SubComments { get; set; }

        // 发布该条评论的用户
        public CommentUserViewModel User { get; set; }
    }
}
