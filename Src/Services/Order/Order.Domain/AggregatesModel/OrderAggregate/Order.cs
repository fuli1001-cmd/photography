using Arise.DDD.Domain.Exceptions;
using Arise.DDD.Domain.SeedWork;
using Photography.Services.Order.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Photography.Services.Order.Domain.AggregatesModel.OrderAggregate
{
    public class Order : Entity, IAggregateRoot
    {
        // 交易参与方一
        public User User1 { get; private set; }
        public Guid User1Id { get; private set; }

        // 交易参与方二
        public User User2 { get; private set; }
        public Guid User2Id { get; private set; }

        public Guid DealId { get; private set; }

        // 交易付款方（交易参与方之一）
        public User Payer { get; private set; }
        public Guid? PayerId { get; private set; }

        public decimal Price { get; private set; }

        public double CreatedTime { get; private set; }

        public double ClosedTime { get; private set; }

        // 约拍时间
        public double AppointedTime { get; private set; }

        public string Text { get; private set; }

        public OrderStatus OrderStatus { get; private set; }

        private readonly List<Attachment> _attachments = null;
        public IReadOnlyCollection<Attachment> Attachments => _attachments;

        #region location properties
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public string LocationName { get; private set; }
        public string Address { get; private set; }
        #endregion

        public Order(Guid user1Id, Guid user2Id, Guid dealId, Guid? payerId, decimal price, double appointedTime, 
            string text, double latitude, double longitude, string locationName, string address)
        {
            User1Id = user1Id;
            User2Id = user2Id;
            DealId = dealId;
            PayerId = payerId;
            Price = price;
            AppointedTime = appointedTime;
            Text = text;
            Latitude = latitude;
            Longitude = longitude;
            LocationName = locationName;
            Address = address;
            CreatedTime = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
            OrderStatus = OrderStatus.Created;
        }

        public void Cancel()
        {
            if (OrderStatus != OrderStatus.Created)
                throw new DomainException("当前订单状态不能取消。");

            OrderStatus = OrderStatus.Canceled;
        }

        public void Reject()
        {
            if (OrderStatus != OrderStatus.Created)
                throw new DomainException("当前订单状态不能拒绝。");

            OrderStatus = OrderStatus.Rejected;
        }

        public void ConfirmShot()
        {
            if (OrderStatus != OrderStatus.WaitingForShooting)
                throw new DomainException("状态错误，设置失败。");

            OrderStatus = OrderStatus.WaitingForUploadOriginal;
        }

        // 上传原片
        public void UploadOriginalFiles(IEnumerable<Attachment> attachments)
        {
            AddAttachments(AttachmentStatus.Original, attachments);
            OrderStatus = OrderStatus.WaitingForSelection;
        }

        // 选择原片
        public void SelectOriginalFiles(IEnumerable<string> attachments)
        {
            _attachments.Where(a => a.AttachmentStatus == AttachmentStatus.Original || a.AttachmentStatus == AttachmentStatus.SelectedOriginal)
                .ToList().ForEach(a =>
                {
                    if (!attachments.Contains(a.Id.ToString()))
                        throw new DomainException("找不到选择的原片。");
                    a.SetAttachmentStatus(AttachmentStatus.SelectedOriginal);
                });

            OrderStatus = OrderStatus.WaitingForUploadProcessed;
        }

        // 上传精修片
        public void UploadProcessedFiles(IEnumerable<Attachment> attachments)
        {
            AddAttachments(AttachmentStatus.Processed, attachments);
            OrderStatus = OrderStatus.WaitingForCheck;
        }

        // 接受精修片
        public void AcceptProcessedFiles()
        {
            OrderStatus = OrderStatus.Finished;
            ClosedTime = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
        }

        private void AddAttachments(AttachmentStatus status, IEnumerable<Attachment> attachments)
        {
            var existedNames = _attachments.Where(a => a.AttachmentStatus == status)
                .Select(a => a.Name);

            // 只添加原来没有的文件，避免重复添加
            foreach (var a in attachments)
            {
                if (!existedNames.Contains(a.Name))
                {
                    a.SetAttachmentStatus(status);
                    _attachments.Add(a);
                }
            }
        }
    }

    // 订单状态
    public enum OrderStatus
    {
        // 已创建
        Created,
        // 待拍片
        WaitingForShooting,
        // 待上传拍摄的照片
        WaitingForUploadOriginal,
        // 待选片
        WaitingForSelection,
        // 待上传处理后的照片
        WaitingForUploadProcessed,
        // 待验收
        WaitingForCheck,
        // 已完成
        Finished,
        // 已取消
        Canceled,
        // 已拒绝
        Rejected
    }
}
