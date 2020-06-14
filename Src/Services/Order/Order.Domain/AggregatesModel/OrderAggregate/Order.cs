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

        public Order()
        {
            CreatedTime = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
            OrderStatus = OrderStatus.Created;
        }

        public Order(Guid user1Id, Guid user2Id, Guid dealId, Guid? payerId, decimal price, double appointedTime, 
            string text, double latitude, double longitude, string locationName, string address)
            : this()
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
        }

        public void Accept()
        {
            if (OrderStatus != OrderStatus.Created) 
                throw new ClientException("操作失败。", new List<string> { "Current order status is not 'Created'." });

            OrderStatus = OrderStatus.WaitingForShooting;
        }

        public void Cancel()
        {
            if (OrderStatus != OrderStatus.Created)
                throw new ClientException("操作失败。", new List<string> { "Current order status is not 'Created'." });

            OrderStatus = OrderStatus.Canceled;
        }

        public void Reject()
        {
            if (OrderStatus != OrderStatus.Created)
                throw new ClientException("操作失败。", new List<string> { "Current order status is not 'Created'." });

            OrderStatus = OrderStatus.Rejected;
        }

        public void ConfirmShot()
        {
            if (OrderStatus != OrderStatus.WaitingForShooting)
                throw new ClientException("操作失败。", new List<string> { "Current order status is not 'WaitingForShooting'." });

            OrderStatus = OrderStatus.WaitingForUploadOriginal;
        }

        // 上传原片
        public void UploadOriginalFiles(IEnumerable<string> attachmentNames)
        {
            if (!CheckOriginalAttachments(attachmentNames))
                throw new ClientException("不能删除已被对方选择的原片。");

            UpdateOriginalAttachments(attachmentNames);

            OrderStatus = OrderStatus.WaitingForSelection;
        }

        // 选择原片
        public void SelectOriginalFiles(IEnumerable<string> attachments)
        {
            // 首先保证选择的附件都属于原片或已选择的原片
            // 把选择的附件状态改为SelectedOriginal
            foreach (var name in attachments)
            {
                var attachment = _attachments.FirstOrDefault(a => a.Name == name && (a.AttachmentStatus == AttachmentStatus.Original || a.AttachmentStatus == AttachmentStatus.SelectedOriginal));
                if (attachment == null)
                    throw new ClientException("操作失败。", new List<string> { "Can't find " + name });
                attachment.SetAttachmentStatus(AttachmentStatus.SelectedOriginal);
            }

            // 把原来是选择的原片但不属于这次选择的改为Original状态
            var selectedAttachments = _attachments.Where(a => a.AttachmentStatus == AttachmentStatus.SelectedOriginal);
            foreach (var attachment in selectedAttachments)
            {
                if (!attachments.Contains(attachment.Name.ToLower()))
                    attachment.SetAttachmentStatus(AttachmentStatus.Original);
            }

            // 把订单改为待上传精修片状态
            OrderStatus = OrderStatus.WaitingForUploadProcessed;
        }

        // 上传精修片
        public void UploadProcessedFiles(IEnumerable<string> attachmentNames)
        {
            UpdateProcessedAttachments(attachmentNames);
            OrderStatus = OrderStatus.WaitingForCheck;
        }

        // 接受精修片
        public void AcceptProcessedFiles()
        {
            OrderStatus = OrderStatus.Finished;
            ClosedTime = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
        }

        // 检查上传的原片是否已被对方选择，不能删掉
        private bool CheckOriginalAttachments(IEnumerable<string> attachmentNames)
        {
            attachmentNames = attachmentNames.Select(n => n.ToLower());
            var selectedOriginalFileNames = _attachments.Where(a => a.AttachmentStatus == AttachmentStatus.SelectedOriginal).Select(a => a.Name.ToLower());
            foreach (var name in selectedOriginalFileNames)
            {
                if (!attachmentNames.Contains(name))
                    return false;
            }
            return true;
        }

        private void UpdateOriginalAttachments(IEnumerable<string> attachmentNames)
        {
            var otherStatusAttachments = _attachments.Where(a => a.AttachmentStatus != AttachmentStatus.Original).ToList();
            var selectedAttachmentNames = _attachments.Where(a => a.AttachmentStatus == AttachmentStatus.SelectedOriginal).Select(a => a.Name.ToLower()).ToList();

            _attachments.Clear();
            _attachments.AddRange(otherStatusAttachments);

            // 上传的原片不能改变已选择原片的状态
            foreach (var name in attachmentNames)
            {
                if (!selectedAttachmentNames.Contains(name.ToLower()))
                    _attachments.Add(new Attachment(name, AttachmentStatus.Original));
            }
        }

        // 客户端每次会把最新的附件列表传上来
        // 因此只使用每次传上来的附件，之前的同状态附件都删掉
        private void UpdateProcessedAttachments(IEnumerable<string> attachmentNames)
        {
            var otherStatusAttachments = _attachments.Where(a => a.AttachmentStatus != AttachmentStatus.Processed).ToList();
            _attachments.Clear();
            _attachments.AddRange(otherStatusAttachments);
            _attachments.AddRange(attachmentNames.Select(name => new Attachment(name, AttachmentStatus.Processed)));
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
