using Arise.DDD.Domain.SeedWork;
using Photography.Services.User.Domain.AggregatesModel.AlbumAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.User.Domain.AggregatesModel.AlbumPhotoAggregate
{
    public class AlbumPhoto : Entity, IAggregateRoot
    {
        public string Name { get; private set; }

        public string DisplayName { get; private set; }

        public double CreatedTime { get; private set; }

        public double UpdatedTime { get; private set; }

        public Guid AlbumId { get; private set; }
        public Album Album { get; private set; }
    }
}
