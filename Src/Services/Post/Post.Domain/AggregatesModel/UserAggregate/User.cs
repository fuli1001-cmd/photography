using Arise.DDD.Domain.SeedWork;
using Photography.Services.Post.Domain.AggregatesModel.CommentAggregate;
using Photography.Services.Post.Domain.AggregatesModel.UserCommentRelationAggregate;
using Photography.Services.Post.Domain.AggregatesModel.UserPostRelationAggregate;
using Photography.Services.Post.Domain.AggregatesModel.UserRelationAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Domain.AggregatesModel.UserAggregate
{
    public class User : Entity, IAggregateRoot
    {
        public string Nickname { get; private set; }
        public string Avatar { get; private set; }
        public UserType? UserType { get; private set; }
        public int Score { get; private set; }

        private readonly List<PostAggregate.Post> _posts = null;
        public IReadOnlyCollection<PostAggregate.Post> Posts => _posts;

        private readonly List<PostAggregate.Post> _appointments = null;
        public IReadOnlyCollection<PostAggregate.Post> Appointments => _appointments;

        private readonly List<Comment> _comments = null;
        public IReadOnlyCollection<Comment> Comments => _comments;

        private readonly List<UserPostRelation> _userPostRelations = null;
        public IReadOnlyCollection<UserPostRelation> UserPostRelations => _userPostRelations;

        private readonly List<UserCommentRelation> _userCommentRelations = null;
        public IReadOnlyCollection<UserCommentRelation> UserCommentRelations => _userCommentRelations;

        // Note: self reference many to many relations can't use field, so use property directly here.
        //private readonly List<UserRelation> _followers;
        //public IReadOnlyCollection<UserRelation> Followers => _followers;

        //private readonly List<UserRelation> _followedUsers;
        //public IReadOnlyCollection<UserRelation> FollowedUsers => _followedUsers;

        public List<UserRelation> Followers { get; private set; }
        public List<UserRelation> FollowedUsers { get; private set; }

        public User() { }

        public User(string id, string nickName)
        {
            Id = Guid.Parse(id);
            Nickname = nickName;
        }

        public void Update(string nickName, string avatar, UserType? userType)
        {
            Nickname = nickName;
            Avatar = avatar;
            UserType = userType;
        }
    }

    public enum UserType
    {
        Photographer,
        Model
    }
}
