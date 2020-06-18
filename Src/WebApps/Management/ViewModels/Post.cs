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

        public double CreatedTime { get; set; }

        public double? UpdatedTime { get; set; }

        public IEnumerable<PostAttachment> PostAttachments { get; set; }

        public User User { get; set; }
    }
}
