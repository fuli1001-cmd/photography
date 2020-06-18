using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.WebApps.Management.ViewModels
{
    public class PostAttachment
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Text { get; set; }

        public AttachmentType AttachmentType { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public string Thumbnail { get; set; }
    }

    public enum AttachmentType
    {
        Image,
        Video
    }
}
