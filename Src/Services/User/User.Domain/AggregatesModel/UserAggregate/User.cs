using Arise.DDD.Domain.SeedWork;
using Photography.Services.User.Domain.AggregatesModel.UserRelationAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Photography.Services.User.Domain.AggregatesModel.UserAggregate
{
    public class User : Entity, IAggregateRoot
    {
        public string UserName { get; private set; }
        public string Nickname { get; private set; }
        public string Phonenumber { get; private set; }
        public string Avatar { get; private set; }
        public bool? Gender { get; private set; }
        public double? Birthday { get; private set; }
        public UserType? UserType { get; private set; }
        public string Province { get; private set; }
        public string City { get; private set; }
        public string Sign { get; private set; }
        public int LikedCount { get; private set; }
        public int FollowingCount { get; private set; }
        public int FollowerCount { get; private set; }
        // 约拍值
        public int Score { get; private set; }
        public string Code { get; private set; }
        public bool RealNameRegistered { get; private set; }
        // ChatServer needed Property
        public int ChatServerUserId { get; private set; }

        public List<UserRelation> Followers { get; private set; }
        public List<UserRelation> FollowedUsers { get; private set; }
    }

    public enum UserType
    {
        Photographer,
        Model
    }
}
