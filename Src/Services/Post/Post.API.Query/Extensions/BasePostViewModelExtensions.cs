using Microsoft.Extensions.Logging;
using Photography.Services.Post.API.Query.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.API.Query.Extensions
{
    public static class BasePostViewModelExtensions
    {
        public static void SetAttachmentProperties(this BasePostViewModel basePost, ILogger logger)
        {
            basePost.PostAttachments.ForEach(a =>
            {
                var sections = a.Name.Split('$');
                try
                {
                    a.Width = int.Parse(sections[1]);
                    a.Height = int.Parse(sections[2]);
                    if (a.AttachmentType == Domain.AggregatesModel.PostAggregate.AttachmentType.Video)
                        a.Thumbnail = a.Name.Substring(0, a.Name.LastIndexOf('.')) + ".jpg";
                }
                catch (Exception ex)
                {
                    logger.LogError("SetAttachmentProperties: {@SetAttachmentPropertiesException}", ex);
                }
            });
        }
    }
}
