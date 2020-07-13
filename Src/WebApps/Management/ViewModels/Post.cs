using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.WebApps.Management.ViewModels
{
    public class Post
    {
        public Guid Id { get; set; }

        public string Text { get; set; }

        public List<PostAttachment> PostAttachments { get; set; }

        public Visibility Visibility { get; set; }

        public User User { get; set; }
    }

    public enum Visibility
    {
        Public,
        Friends,
        Password,
        SelectedFriends
    }
}
