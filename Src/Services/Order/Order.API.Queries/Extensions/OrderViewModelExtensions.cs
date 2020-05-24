using Microsoft.Extensions.Logging;
using Photography.Services.Order.API.Query.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Order.API.Query.Extensions
{
    public static class OrderViewModelExtensions
    {
        public static void SetAttachmentProperties(this OrderViewModel order, ILogger logger)
        {
            foreach (var a in order.Attachments)
            {
                var sections = a.Name.Split('$');
                try
                {
                    a.Width = int.Parse(sections[1]);
                    a.Height = int.Parse(sections[2]);
                }
                catch (Exception ex)
                {
                    logger.LogError("SetAttachmentProperties: {@SetAttachmentPropertiesException}", ex);
                }
            }
        }
    }
}
