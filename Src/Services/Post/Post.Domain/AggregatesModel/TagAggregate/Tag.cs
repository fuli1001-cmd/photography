using Arise.DDD.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Domain.AggregatesModel.TagAggregate
{
    public class Tag : Entity, IAggregateRoot
    {
        // 标签名
        public string Name { get; private set; }

        // 标签使用数量
        public int Count { get; set; }
    }
}
