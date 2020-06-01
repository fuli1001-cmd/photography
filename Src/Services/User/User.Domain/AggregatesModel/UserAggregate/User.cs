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
        public string BackgroundImage { get; private set; }
        public Gender? Gender { get; private set; }
        public double? Birthday { get; private set; }
        public UserType? UserType { get; private set; }
        public string Province { get; private set; }
        public string City { get; private set; }
        public string Sign { get; private set; }

        // 关注的用户数量
        public int FollowingCount { get; private set; }

        // 关注者数量
        public int FollowerCount { get; private set; }

        // 发布的帖子数量
        public int PostCount { get; private set; }

        // 发布的约拍数量
        public int AppointmentCount { get; private set; }

        // 被赞数量
        public int LikedCount { get; private set; }

        // 赞过的帖子数量
        public int LikedPostCount { get; private set; }

        // 约拍值
        public int Score { get; private set; }
        public string Code { get; private set; }
        public RealNameRegistrationStatus RealNameRegistrationStatus { get; private set; }
        // ChatServer needed Property
        public int ChatServerUserId { get; private set; }

        public List<UserRelation> Followers { get; private set; }
        public List<UserRelation> FollowedUsers { get; private set; }

        public User()
        {
            
        }

        public User(string id, string userName, string phonenumber, string code, string nickName)
        {
            Id = Guid.Parse(id);
            UserName = userName;
            Phonenumber = phonenumber;
            Code = code;
            Nickname = nickName;
        }

        public void Update(string nickname, Gender? gender, double? birthday, UserType? userType, 
            string province, string city, string sign, string avatar)
        {
            Nickname = nickname;
            Gender = gender;
            Birthday = birthday;
            UserType = userType;
            Province = province;
            City = city;
            Sign = sign;
            Avatar = avatar;
        }

        public void UpdateBackground(string background)
        {
            BackgroundImage = background;
        }

        public void IncreasePostCount()
        {
            PostCount++;
        }

        public void DecreasePostCount()
        {
            PostCount = Math.Max(0, PostCount - 1);
        }

        public void IncreaseFollowerCount()
        {
            FollowerCount++;
        }

        public void DecreaseFollowerCount()
        {
            FollowerCount = Math.Max(0, FollowerCount - 1);
        }

        public void IncreaseFollowingCount()
        {
            FollowingCount++;
        }

        public void DecreaseFollowingCount()
        {
            FollowingCount = Math.Max(0, FollowingCount - 1);
        }
    }

    public enum UserType
    {
        Photographer,
        Model
    }

    public enum Gender
    {
        Female,
        Male
    }

    public enum RealNameRegistrationStatus
    {
        NotRegister,
        Authenticating,
        Authenticated
    }
}
