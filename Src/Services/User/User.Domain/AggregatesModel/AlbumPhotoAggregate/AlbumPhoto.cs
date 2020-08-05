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

        public AlbumPhoto()
        {
            CreatedTime = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
            UpdatedTime = CreatedTime;
        }

        public AlbumPhoto(string name, string displayName) : this()
        {
            Name = name;
            DisplayName = displayName;
        }

        public AlbumPhoto MoveToAlbum(Guid albumId)
        {
            var albumPhoto = new AlbumPhoto();

            albumPhoto.Name = Name;
            albumPhoto.DisplayName = DisplayName;
            albumPhoto.AlbumId = albumId;
            albumPhoto.CreatedTime = CreatedTime;
            albumPhoto.UpdatedTime = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

            return albumPhoto;
        }

        public void UpdateDisplayName(string displayName)
        {
            DisplayName = displayName;
            UpdatedTime = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
        }
    }
}
