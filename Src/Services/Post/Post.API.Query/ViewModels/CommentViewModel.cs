using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.API.Query.ViewModels
{
    public class CommentViewModel
    {
        public string Text { get; set; }
        public int Likes { get; set; }
        public double CreatedTime { get; private set; }
        public IEnumerable<CommentViewModel> SubComments { get; set; }
        public CommentUserViewModel User { get; set; }
    }
}
