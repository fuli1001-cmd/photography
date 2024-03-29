﻿using Arise.DDD.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Domain.AggregatesModel.PostAggregate
{
    public class PostAttachment : Entity
    {
        public string Name { get; private set; }
        public string Text { get; private set; }
        public AttachmentType AttachmentType { get; private set; }
        public AttachmentStatus? AttachmentStatus { get; private set; }

        // 仅自己可见
        public bool IsPrivate { get; private set; }

        public Post Post { get; private set; }
        public Guid PostId { get; private set; }

        public PostAttachment(string name, string text, AttachmentType attachmentType, bool isPrivate = false)
        {
            Name = name;
            Text = text;
            AttachmentType = attachmentType;
            IsPrivate = isPrivate;
        }

        public void SetAttachmentStatus(AttachmentStatus type)
        {
            AttachmentStatus = type;
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
        Processed
    }
}
