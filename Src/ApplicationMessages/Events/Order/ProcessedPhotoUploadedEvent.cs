using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationMessages.Events.Order
{
    // 精修片已上传事件
    public class ProcessedPhotoUploadedEvent : BaseEvent
    {
        // 订单里上传原片的用户id
        public Guid UploadPhotoUserId { get; set; }

        // 订单里另一个用户的id
        public Guid AnotherUserId { get; set; }

        // 订单id
        public Guid OrderId { get; set; }
    }
}
