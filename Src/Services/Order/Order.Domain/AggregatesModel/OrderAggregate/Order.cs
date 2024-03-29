﻿using Arise.DDD.Domain.Exceptions;
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
        public UserType User1Type { get; set; }

        // 交易参与方二
        public User User2 { get; private set; }
        public Guid User2Id { get; private set; }
        public UserType User2Type { get; set; }

        public Guid DealId { get; private set; }

        // 约拍对象（User2）类型
        public UserType AppointmentedUserType { get; private set; }

        // 支付类型（User1的视角）
        public int PayerType { get; private set; }

        // 交易付款方（交易参与方之一）
        public User Payer { get; private set; }
        public Guid? PayerId { get; private set; }

        public decimal Price { get; private set; }

        public double CreatedTime { get; private set; }

        public double UpdatedTime { get; private set; }

        public double? ClosedTime { get; private set; }

        // 约拍时间
        public double AppointedTime { get; private set; }

        public string Text { get; private set; }

        public OrderStatus OrderStatus { get; private set; }

        // 取消或拒绝说明
        public string Description { get; private set; }

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
            UpdatedTime = CreatedTime;
            OrderStatus = OrderStatus.WaitingForConfirm;
        }

        public Order(Guid user1Id, Guid user2Id, UserType user1Type, UserType user2Type, Guid dealId, UserType appointmentedUserType, int payerType, 
            Guid? payerId, decimal price, double appointedTime, string text, double latitude, double longitude, string locationName, string address)
            : this()
        {
            User1Id = user1Id;
            User2Id = user2Id;
            User1Type = user1Type;
            User2Type = user2Type;
            DealId = dealId;
            AppointmentedUserType = appointmentedUserType;
            PayerType = payerType; 
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
            if (OrderStatus != OrderStatus.WaitingForConfirm) 
                throw new ClientException("操作失败", new List<string> { "Current order status is not 'Created'." });

            OrderStatus = OrderStatus.WaitingForShooting;
            UpdatedTime = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
        }

        public void Cancel(string description)
        {
            if (OrderStatus != OrderStatus.WaitingForConfirm)
                throw new ClientException("操作失败", new List<string> { "Current order status is not 'Created'." });

            OrderStatus = OrderStatus.Canceled;
            Description = description;
            UpdatedTime = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
            ClosedTime = UpdatedTime;
        }

        public void Reject(string description)
        {
            if (OrderStatus != OrderStatus.WaitingForConfirm)
                throw new ClientException("操作失败", new List<string> { "Current order status is not 'Created'." });

            OrderStatus = OrderStatus.Rejected;
            Description = description;
            UpdatedTime = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
            ClosedTime = UpdatedTime;
        }

        public void ConfirmShot()
        {
            if (OrderStatus != OrderStatus.WaitingForShooting)
                throw new ClientException("操作失败", new List<string> { "Current order status is not 'WaitingForShooting'." });

            OrderStatus = OrderStatus.WaitingForUploadOriginal;
            UpdatedTime = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
        }

        // 上传原片
        public void UploadOriginalFiles(IEnumerable<string> attachmentNames)
        {
            if (!CheckOriginalAttachments(attachmentNames))
                throw new ClientException("不能删除已被对方选择的原片");

            UpdateOriginalAttachments(attachmentNames);

            OrderStatus = OrderStatus.WaitingForSelection;
            UpdatedTime = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
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
                    throw new ClientException("操作失败", new List<string> { "Can't find " + name });
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
            UpdatedTime = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
        }

        // 上传精修片
        public void UploadProcessedFiles(IEnumerable<string> attachmentNames)
        {
            UpdateProcessedAttachments(attachmentNames);
            OrderStatus = OrderStatus.WaitingForCheck;
            UpdatedTime = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
        }

        // 接受精修片
        public void AcceptProcessedFiles()
        {
            OrderStatus = OrderStatus.Finished;
            UpdatedTime = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
            ClosedTime = UpdatedTime;
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
        // 待确认
        WaitingForConfirm,
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

    public enum OrderStage
    {
        Shooting, // 待拍片阶段
        Selection, // 待选片阶段
        Production, // 待出片阶段
        Finished // 已完成阶段
    }
}
