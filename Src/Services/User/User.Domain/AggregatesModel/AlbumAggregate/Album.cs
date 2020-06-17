using Arise.DDD.Domain.SeedWork;
using Photography.Services.User.Domain.AggregatesModel.AlbumPhotoAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.User.Domain.AggregatesModel.AlbumAggregate
{
    public class Album : Entity, IAggregateRoot
    {
        public string Name { get; private set; }

        public double CreatedTime { get; private set; }

        public double UpdatedTime { get; private set; }

        public Guid UserId { get; private set; }
        public UserAggregate.User User { get; private set; }

        private readonly List<AlbumPhoto> _albumPhotos = null;
        public IReadOnlyCollection<AlbumPhoto> AlbumPhotos => _albumPhotos;

        public Album()
        {
            CreatedTime = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
            UpdatedTime = CreatedTime;
        }

        public Album(string name, Guid userId) : this()
        {
            Name = name;
            UserId = userId;
        }

        public void Update(string name)
        {
            Name = name;
            UpdatedTime = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
        }

        public void Delete()
        {
            _albumPhotos.Clear();
        }
    }
}
