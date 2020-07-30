using AutoMapper;
using Photography.Services.User.API.Query.Interfaces;
using Photography.Services.User.API.Query.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using Photography.Services.User.Domain.AggregatesModel.UserRelationAggregate;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Arise.DDD.API.Paging;
using Microsoft.AspNetCore.Http;

namespace Photography.Services.User.Infrastructure.Queries
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

        public async Task<UserViewModel> GetCurrentUserAsync()
        {
            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var queryableUsers = from u in _identityContext.Users
                                 where u.Id == myId
                                 select u;

            var user = await GetUserViewModels(queryableUsers).FirstOrDefaultAsync();

            user.Age = GetAge(user.Birthday);

            return user;
        }

        public async Task<UserViewModel> GetUserAsync(Guid? userId, int? oldUserId, string nickName)
        {
            IQueryable<Domain.AggregatesModel.UserAggregate.User> queryableUsers = null;

            if (userId != null)
                queryableUsers = from u in _identityContext.Users where u.Id == userId select u;
            else if (!string.IsNullOrWhiteSpace(nickName))
                queryableUsers = from u in _identityContext.Users where u.Nickname.ToLower() == nickName.ToLower() select u;
            else if (oldUserId != null)
                queryableUsers = from u in _identityContext.Users where u.ChatServerUserId == oldUserId select u;
            else
                return null;

            var user = await GetUserViewModels(queryableUsers).FirstOrDefaultAsync();

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

            var followers = await (from u in _identityContext.Users
                             join r1 in _identityContext.UserRelations
                             on new { FollowerId = u.Id, FollowedUserId = userId } equals new { FollowerId = r1.FollowerId, FollowedUserId = r1.FollowedUserId }
                             select new FollowerViewModel
                             {
                                 Id = u.Id,
                                 Nickname = u.Nickname,
                                 Avatar = u.Avatar,
                                 FollowersCount = u.FollowerCount,
                                 PostCount = u.PostCount
                             }).ToListAsync();

            var MyFollowedUserIds = await _identityContext.UserRelations.Where(r => r.FollowerId == myId).Select(r => r.FollowedUserId).ToListAsync();

            followers.ForEach(f => f.Followed = MyFollowedUserIds.Contains(f.Id));

            return followers;

            // TODO: invistgate why below code does not work?
            //return await (from u in _identityContext.Users
            //              join r1 in _identityContext.UserRelations
            //              on new { FollowerId = u.Id, FollowedUserId = userId } equals new { FollowerId = r1.FollowerId, FollowedUserId = r1.FollowedUserId }
            //              join r2 in _identityContext.UserRelations
            //              on new { FollowerId = myId, FollowedUserId = u.Id } equals new { FollowerId = r2.FollowerId, FollowedUserId = r2.FollowedUserId }
            //              into myFollows
            //              select new FollowerViewModel
            //              {
            //                  Id = u.Id,
            //                  Nickname = u.Nickname,
            //                  Avatar = u.Avatar,
            //                  Followed = myFollows.Any() // 表示当前登录用户是否关注了这个人
            //              }).ToListAsync();
        }

        /// <summary>
        /// 获取用户关注的人
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<FollowerViewModel>> GetFollowedUsersAsync(Guid userId)
        {
            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var followedUsers = await (from u in _identityContext.Users
                                       join r in _identityContext.UserRelations
                                       on new { FollowerId = userId, FollowedUserId = u.Id } equals new { FollowerId = r.FollowerId, FollowedUserId = r.FollowedUserId }
                                       select new FollowerViewModel
                                       {
                                           Id = u.Id,
                                           Nickname = u.Nickname,
                                           Avatar = u.Avatar,
                                           Followed = true,
                                           FollowersCount = u.FollowerCount,
                                           PostCount = u.PostCount
                                       }).ToListAsync();

            if (myId != userId)
            {
                var MyFollowedUserIds = await _identityContext.UserRelations.Where(r => r.FollowerId == myId).Select(r => r.FollowedUserId).ToListAsync();

                followedUsers.ForEach(f => f.Followed = MyFollowedUserIds.Contains(f.Id));
            }

            return followedUsers;
        }

        public async Task<List<UserSearchResult>> SearchUsersAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
                return new List<UserSearchResult>();

            var users = await _identityContext.Users.Where(u => u.Nickname.ToLower().Contains(key.ToLower()))
                .Select(u => new UserSearchResult
                {
                    Id = u.Id,
                    Nickname = u.Nickname,
                    Avatar = u.Avatar,
                    PostCount = u.PostCount,
                    FollowerCount = u.FollowerCount
                }).ToListAsync();

            if (users.Count > 0)
            {
                var claim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                var myId = claim == null ? Guid.Empty : Guid.Parse(claim.Value);
                var MyFollowedUserIds = await _identityContext.UserRelations.Where(r => r.FollowerId == myId).Select(r => r.FollowedUserId).ToListAsync();
                users.ForEach(f => f.Followed = MyFollowedUserIds.Contains(f.Id));
            }

            return users;
        }

        /// <summary>
        /// 获取管理平台使用的用户数据
        /// </summary>
        /// <param name="pagingParameters"></param>
        /// <returns></returns>
        public async Task<PagedList<ExaminingUserViewModel>> GetExaminingUsersAsync(PagingParameters pagingParameters)
        {
            var queryableDto = from u in _identityContext.Users
                                 orderby u.Nickname
                                 select new ExaminingUserViewModel
                                 {
                                     Id = u.Id,
                                     Nickname = u.Nickname,
                                     Avatar = u.Avatar,
                                     Sign = u.Sign,
                                     BackgroundImage = u.BackgroundImage,
                                     Gender = u.Gender,
                                     Birthday = u.Birthday,
                                     UserType = u.UserType,
                                     Province = u.Province,
                                     City = u.City,
                                     IdCardBack = u.IdCardBack,
                                     IdCardFront = u.IdCardFront,
                                     IdCardHold = u.IdCardHold,
                                     Disabled = u.DisabledTime == null ? false : DateTime.UtcNow < u.DisabledTime.Value,
                                     RealNameRegistrationStatus = u.RealNameRegistrationStatus
                                 };

            return await PagedList<ExaminingUserViewModel>.ToPagedListAsync(queryableDto, pagingParameters);
        }

        private IQueryable<UserViewModel> GetUserViewModels(IQueryable<Domain.AggregatesModel.UserAggregate.User> queryableUsers)
        {
            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            return from u in queryableUsers
                   select new UserViewModel
                   {
                       Id = u.Id,
                       Code = u.Code,
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
                       OngoingOrderCount = u.OngoingOrderCount,
                       ShootingStageOrderCount = u.ShootingStageOrderCount,
                       SelectionStageOrderCount = u.SelectionStageOrderCount,
                       ProductionStageOrderCount = u.ProductionStageOrderCount,
                       LikedCount = u.LikedCount,
                       FollowingCount = u.FollowingCount,
                       FollowerCount = u.FollowerCount,
                       AppointmentCount = u.AppointmentCount,
                       LikedPostCount = u.LikedPostCount,
                       Score = u.Score,
                       Phonenumber = u.Phonenumber,
                       PostCount = u.PostCount,
                       ChatServerUserId = u.ChatServerUserId,
                       ViewFollowedUsersAllowed = u.ViewFollowedUsersAllowed,
                       ViewFollowersAllowed = u.ViewFollowersAllowed,
                       RealNameRegistrationStatus = u.RealNameRegistrationStatus,
                       Followed = (from ur in _identityContext.UserRelations
                                   where ur.FollowerId == myId && ur.FollowedUserId == u.Id
                                   select ur.Id).Count() > 0
                   };
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
