using Arise.DDD.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Order.Domain.AggregatesModel.OrderAggregate
{
    public class Attachment : Entity
    {
        public string Name { get; private set; }
        public string Text { get; private set; }
        public AttachmentType AttachmentType { get; private set; }
        public AttachmentStatus AttachmentStatus { get; private set; }

        public Order Order { get; private set; }
        public Guid OrderId { get; private set; }

        public Attachment(string name)
        {
            Name = name;
            // 目前订单附件只有图片
            AttachmentType = AttachmentType.Image;
        }

        public void SetAttachmentStatus(AttachmentStatus status)
        {
            AttachmentStatus = status;
        }
    }

    public enum AttachmentType
    {
        Image,
        Video
    }

    public enum AttachmentStatus
    {
        Original,
        SelectedOriginal,
        Processed
    }
}
