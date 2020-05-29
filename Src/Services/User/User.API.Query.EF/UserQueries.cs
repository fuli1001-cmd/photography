using AutoMapper;
using Microsoft.AspNetCore.Http;
using Photography.Services.User.API.Query.Interfaces;
using Photography.Services.User.API.Query.ViewModels;
using Photography.Services.User.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using Photography.Services.User.Domain.AggregatesModel.UserAggregate;
using Photography.Services.User.Domain.AggregatesModel.UserRelationAggregate;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Query.EF
{
    public class UserQueries : IUserQueries
    {
        private readonly UserContext _identityContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly ILogger<UserQueries> _logger;

        public UserQueries(UserContext identityContext, IHttpContextAccessor httpContextAccessor, IMapper mapper, ILogger<UserQueries> logger)
        {
            _identityContext = identityContext ?? throw new ArgumentNullException(nameof(identityContext));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public MeViewModel GetCurrentUserAsync()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = _identityContext.Users.SingleOrDefault(u => u.Id.ToString() == userId);
            return _mapper.Map<MeViewModel>(user);
        }

        public UserViewModel GetUserAsync(Guid? userId, int? oldUserId, string nickName)
        {
            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            IQueryable<Domain.AggregatesModel.UserAggregate.User> queryableUsers = null;

            if (userId != null)
                queryableUsers = from u in _identityContext.Users where u.Id == userId select u;
            else if (nickName != null)
                queryableUsers = from u in _identityContext.Users where u.Nickname.ToLower() == nickName.ToLower() select u;
            else if (oldUserId != null)
                queryableUsers = from u in _identityContext.Users where u.ChatServerUserId == oldUserId select u;
            else
                return null;

            var user = (from u in queryableUsers
                        select new UserViewModel
                        {
                            Id = u.Id,
                            Nickname = u.Nickname,
                            Avatar = u.Avatar,
                            UserType = u.UserType,
                            UserName = u.UserName,
                            Gender = u.Gender,
                            Birthday = u.Birthday,
                            Province = u.Province,
                            City = u.City,
                            Sign = u.Sign,
                            LikedCount = u.LikedCount,
                            FollowingCount = u.FollowingCount,
                            FollowerCount = u.FollowerCount,
                            Score = u.Score,
                            Phonenumber = u.Phonenumber,
                            PostCount = u.PostCount,
                            ChatServerUserId = u.ChatServerUserId,
                            Followed = (from ur in _identityContext.UserRelations
                                        where ur.FollowerId == myId && ur.FollowedUserId == userId
                                        select ur.Id).Count() > 0
                        }).SingleOrDefault();

            return user;
        }

        public List<FriendViewModel> GetFriendsAsync()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var userRelations = GetFriends(userId);
            var friends = _identityContext.Users
                .Where(u => userRelations.Select(ur => ur.FollowedUserId).Contains(u.Id));
            var friendViewModels = _mapper.Map<List<FriendViewModel>>(friends);

            friendViewModels.ForEach(f => f.Muted = userRelations.SingleOrDefault(ur => ur.FollowedUserId == f.Id)?.MutedFollowedUser ?? false);

            return friendViewModels;
        }

        private IQueryable<UserRelation> GetFriends(string userId)
        {
            var friendsQuery = from ur1 in _identityContext.UserRelations
                               join ur2 in _identityContext.UserRelations on
                               new { C1 = ur1.FollowerId, C2 = ur1.FollowedUserId }
                               equals
                               new { C1 = ur2.FollowedUserId, C2 = ur2.FollowerId }
                               where ur1.FollowerId.ToString() == userId
                               select ur1;

            return friendsQuery;
        }

        private IQueryable<Guid> GetFollowedUserIds(string userId)
        {
            return _identityContext.UserRelations
                .Where(ur => ur.FollowerId.ToString() == userId)
                .Select(ur => ur.FollowedUserId);
        }
    }
}
