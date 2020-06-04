using Photography.Services.Post.API.Query.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.API.Query.Extensions
{
    public static class PostAttahmentViewModelExtensions
    {
        public static void SetProperties(this PostAttachmentViewModel attachment)
        {
            var sections = attachment.Name.Split('$');
            try
            {
                attachment.Width = int.Parse(sections[1]);
                attachment.Height = int.Parse(sections[2]);
                if (attachment.AttachmentType == Domain.AggregatesModel.PostAggregate.AttachmentType.Video)
                    attachment.Thumbnail = attachment.Name.Substring(0, attachment.Name.LastIndexOf('.')) + ".jpg";
            }
            catch 
            {
                
            }
        }
    }
}
