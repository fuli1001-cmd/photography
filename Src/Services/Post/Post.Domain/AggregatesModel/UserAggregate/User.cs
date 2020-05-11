using Photography.Services.Post.Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Domain.AggregatesModel.UserAggregate
{
    public class User : Entity, IAggregateRoot
    {
        public string Nickname { get; private set; }
        public string Phonenumber { get; private set; }
        public string Avatar { get; private set; }
        public bool Gender { get; private set; }
        public DateTime Birthday { get; private set; }
        public UserType UserType { get; private set; }
        public string Province { get; private set; }
        public string City { get; private set; }
        public string Sign { get; private set; }
        public int LikedCount { get; private set; }
        public int FollowingCount { get; private set; }
        public int FollowerCount { get; private set; }
        // 约拍值
        public int Points { get; private set; }
        public string Code { get; private set; }
        public bool RealNameRegistered { get; private set; }

        private readonly List<PostAggregate.Post> _posts;
        public IReadOnlyCollection<PostAggregate.Post> Posts => _posts;
    }

    public enum UserType
    {
        Unknown,
        Photographer,
        Model
    }
}
