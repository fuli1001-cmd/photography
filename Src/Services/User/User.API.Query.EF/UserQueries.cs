﻿using AutoMapper;
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

        public async Task<MeViewModel> GetCurrentUserAsync()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = await _identityContext.Users.SingleOrDefaultAsync(u => u.Id.ToString() == userId);
            return _mapper.Map<MeViewModel>(user);
        }

        public async Task<UserViewModel> GetUserAsync(Guid? userId, int? oldUserId, string nickName)
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

            var user = await (from u in queryableUsers
                        select new UserViewModel
                        {
                            Id = u.Id,
                            Nickname = u.Nickname,
                            Avatar = u.Avatar,
                            BackgroundImage = u.BackgroundImage,
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
                        }).SingleOrDefaultAsync();

            user.Age = GetAge(user.Birthday);

            return user;
        }

        public async Task<List<FriendViewModel>> GetFriendsAsync()
        {
            var userId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userRelations = GetFriends(userId);
            var friends = await _identityContext.Users
                .Where(u => userRelations.Select(ur => ur.FollowedUserId).Contains(u.Id))
                .ToListAsync();
            var friendViewModels = _mapper.Map<List<FriendViewModel>>(friends);

            friendViewModels.ForEach(f => f.Muted = userRelations.SingleOrDefault(ur => ur.FollowedUserId == f.Id)?.MutedFollowedUser ?? false);

            return friendViewModels;
        }

        /// <summary>
        /// 获取关注指定用户的人
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<FollowerViewModel>> GetFollowersAsync(Guid userId)
        {
            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            return await (from u in _identityContext.Users
                          join r1 in _identityContext.UserRelations
                          on new { FollowerId = u.Id, FollowedUserId = userId } equals new { FollowerId = r1.FollowerId, FollowedUserId = r1.FollowedUserId }
                          join r2 in _identityContext.UserRelations
                          on new { FollowerId = myId, FollowedUserId = u.Id } equals new { FollowerId = r2.FollowerId, FollowedUserId = r2.FollowedUserId }
                          into myFollows
                          select new FollowerViewModel
                          {
                              Id = u.Id,
                              Nickname = u.Nickname,
                              Avatar = u.Avatar,
                              Followed = myFollows.Any() // 表示当前登录用户是否关注了这个人
                          }).ToListAsync();
        }

        /// <summary>
        /// 获取用户关注的人
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<FollowerViewModel>> GetFollowedUsersAsync(Guid userId)
        {
            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var query = from u in _identityContext.Users
                        join r in _identityContext.UserRelations
                        on new { FollowerId = userId, FollowedUserId = u.Id } equals new { FollowerId = r.FollowerId, FollowedUserId = r.FollowedUserId }
                        select new FollowerViewModel
                        {
                            Id = u.Id,
                            Nickname = u.Nickname,
                            Avatar = u.Avatar,
                            Followed = true
                        };

            if (myId != userId)
                query = from q in query
                        join r in _identityContext.UserRelations
                        on new { FollowerId = myId, FollowedUserId = q.Id } equals new { FollowerId = r.FollowerId, FollowedUserId = r.FollowedUserId }
                        into myFollows
                        select new FollowerViewModel
                        {
                            Id = q.Id,
                            Nickname = q.Nickname,
                            Avatar = q.Avatar,
                            Followed = myFollows.Any() // 表示当前登录用户是否关注了这个人
                        };

            return await query.ToListAsync();
        }

        private IQueryable<UserRelation> GetFriends(Guid userId)
        {
            var friendsQuery = from ur1 in _identityContext.UserRelations
                               join ur2 in _identityContext.UserRelations on
                               new { C1 = ur1.FollowerId, C2 = ur1.FollowedUserId }
                               equals
                               new { C1 = ur2.FollowedUserId, C2 = ur2.FollowerId }
                               where ur1.FollowerId == userId
                               select ur1;

            return friendsQuery;
        }

        private int? GetAge(double? secondsOfBirthday)
        {
            if (secondsOfBirthday == null)
                return null;

            var birthday = DateTime.UnixEpoch.AddSeconds(secondsOfBirthday.Value);

            // 如果当前月份小于生日月份，即当年还未过生，年龄为年份差值再减一岁
            if (DateTime.Now.Month < birthday.Month)
                return Math.Max(DateTime.Now.Year - birthday.Year - 1, 0);

            // 如果当前月份等于生日月份，但是日期还未到生日那天，即当年也还未过生，年龄也为年份差值再减一岁
            if (DateTime.Now.Month == birthday.Month && DateTime.Now.Day < birthday.Day)
                return Math.Max(DateTime.Now.Year - birthday.Year - 1, 0);

            // 其它情况为当年已过生，年龄为年份差值
            return Math.Max(DateTime.Now.Year - birthday.Year, 0);
        }
    }
}
