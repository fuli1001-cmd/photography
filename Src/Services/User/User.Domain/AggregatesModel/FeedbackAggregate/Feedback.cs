using Arise.DDD.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.User.Domain.AggregatesModel.FeedbackAggregate
{
    public class Feedback : Entity, IAggregateRoot
    {
        // 反馈信息描述
        public string Text { get; private set; }

        // 图片1
        public string Image1 { get; private set; }

        // 图片2
        public string Image2 { get; private set; }

        // 图片3
        public string Image3 { get; private set; }

        public double CreatedTime { get; private set; }

        // 所属用户id
        public Guid UserId { get; private set; }

        public UserAggregate.User User { get; private set; }

        public Feedback()
        {
            CreatedTime = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
        }

        public Feedback(string text, string image1, string image2, string image3, Guid userId) : this()
        {
            Text = text;
            Image1 = image1;
            Image2 = image2;
            Image3 = image3;
            UserId = userId;
        }
    }
}
