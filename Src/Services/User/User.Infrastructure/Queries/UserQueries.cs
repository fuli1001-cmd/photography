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
using Arise.DDD.Domain.Exceptions;

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

            if (user != null)
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

            if (user != null)
                user.Age = GetAge(user.Birthday);

            return user;
        }

        public async Task<List<FriendViewModel>> GetFriendsAsync()
        {
            var userId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var friendRelations = GetFriendRelations(userId);

            return await (from u in _identityContext.Users
                          join ur in friendRelations
                          on u.Id equals ur.ToUserId
                          orderby u.Nickname
                          select new FriendViewModel
                          {
                              Id = u.Id,
                              Avatar = u.Avatar,
                              ChatServerUserId = u.ChatServerUserId,
                              Nickname = u.Nickname,
                              Phonenumber = u.Phonenumber,
                              Muted = ur.Muted
                          })
                          .ToListAsync();
        }

        /// <summary>
        /// 获取关注指定用户的人
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pagingParameters"></param>
        /// <returns></returns>
        public async Task<PagedList<FollowerViewModel>> GetFollowersAsync(Guid userId, PagingParameters pagingParameters)
        {
            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var queryableDto = from u in _identityContext.Users
                               join ur in _identityContext.UserRelations
                               on new { FollowerId = u.Id, FollowedUserId = userId } equals new { FollowerId = ur.FromUserId, FollowedUserId = ur.ToUserId }
                               where ur.Followed
                               orderby ur.FollowTime descending
                               select new FollowerViewModel
                               {
                                   Id = u.Id,
                                   Nickname = u.Nickname,
                                   Avatar = u.Avatar,
                                   FollowersCount = u.FollowerCount,
                                   PostCount = u.PostCount,
                                   OrgAuthStatus = u.OrgAuthStatus,
                                   Followed = _identityContext.UserRelations.Any(ur2 => ur2.FromUserId == myId && ur2.ToUserId == u.Id && ur2.Followed)
                               };

            return await PagedList<FollowerViewModel>.ToPagedListAsync(queryableDto, pagingParameters);
        }

        /// <summary>
        /// 获取用户关注的人
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pagingParameters"></param>
        /// <returns></returns>
        public async Task<PagedList<FollowerViewModel>> GetFollowedUsersAsync(Guid userId, PagingParameters pagingParameters)
        {
            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var queryableDto = from u in _identityContext.Users
                               join ur in _identityContext.UserRelations
                               on new { FollowerId = userId, FollowedUserId = u.Id } equals new { FollowerId = ur.FromUserId, FollowedUserId = ur.ToUserId }
                               where ur.Followed
                               orderby ur.FollowTime descending
                               select new FollowerViewModel
                               {
                                   Id = u.Id,
                                   Nickname = u.Nickname,
                                   Avatar = u.Avatar,
                                   FollowersCount = u.FollowerCount,
                                   PostCount = u.PostCount,
                                   OrgAuthStatus = u.OrgAuthStatus,
                                   Followed = myId == userId ? true : _identityContext.UserRelations.Any(ur2 => ur2.FromUserId == myId && ur2.ToUserId == u.Id && ur2.Followed),
                               };

            return await PagedList<FollowerViewModel>.ToPagedListAsync(queryableDto, pagingParameters);
        }

        public async Task<List<UserSearchResult>> SearchUsersAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
                return new List<UserSearchResult>();

            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var users = await _identityContext.Users.Where(u => u.Nickname.ToLower().Contains(key.ToLower()))
                .Select(u => new UserSearchResult
                {
                    Id = u.Id,
                    Nickname = u.Nickname,
                    Avatar = u.Avatar,
                    PostCount = u.PostCount,
                    FollowerCount = u.FollowerCount,
                    OrgAuthStatus = u.OrgAuthStatus,
                    Followed = _identityContext.UserRelations.Any(ur => ur.FromUserId == myId && ur.ToUserId == u.Id && ur.Followed)
                }).ToListAsync();

            //if (users.Count > 0)
            //{
            //    var claim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            //    var myId = claim == null ? Guid.Empty : Guid.Parse(claim.Value);
            //    var MyFollowedUserIds = await _identityContext.UserRelations.Where(r => r.FollowerId == myId).Select(r => r.FollowedUserId).ToListAsync();
            //    users.ForEach(f => f.Followed = MyFollowedUserIds.Contains(f.Id));
            //}

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
                                     RealNameRegistrationStatus = u.RealNameRegistrationStatus,
                                     OrgAuthStatus = u.OrgAuthStatus
                                 };

            return await PagedList<ExaminingUserViewModel>.ToPagedListAsync(queryableDto, pagingParameters);
        }

        private IQueryable<UserViewModel> GetUserViewModels(IQueryable<Domain.AggregatesModel.UserAggregate.User> queryableUsers)
        {
            var myId = Guid.Empty;
            var claim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
                myId = Guid.Parse(claim.Value);

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
                       OrgDesc = u.OrgDesc,
                       OrgAuthStatus = u.OrgAuthStatus,
                       Followed = myId == Guid.Empty ? false : _identityContext.UserRelations.Any(ur => ur.FromUserId == myId && ur.ToUserId == u.Id && ur.Followed)
                   };
        }

        private IQueryable<UserRelation> GetFriendRelations(Guid userId)
        {
            var friendsQuery = from ur1 in _identityContext.UserRelations
                               join ur2 in _identityContext.UserRelations on
                               new { C1 = ur1.FromUserId, C2 = ur1.ToUserId }
                               equals
                               new { C1 = ur2.ToUserId, C2 = ur2.FromUserId }
                               where ur1.FromUserId == userId && ur1.Followed && ur2.Followed
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

        public async Task<UserOrgAuthInfoViewModel> GetUserOrgAuthInfoAsync(Guid? userId = null)
        {
            var role = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
            if (role == "admin")
            {
                if (userId == null)
                    throw new ClientException("操作失败", new List<string> { "Need user Id." });
            }
            else
            {
                userId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            }

            var result = await (from u in _identityContext.Users
                          where u.Id == userId
                          select new UserOrgAuthInfoViewModel
                          {
                              OrgType = u.OrgType,
                              OrgSchoolName = u.OrgSchoolName,
                              OrgName = u.OrgName,
                              OrgDesc = u.OrgDesc,
                              OrgOperatorName = u.OrgOperatorName,
                              OrgOperatorPhoneNumber = u.OrgOperatorPhoneNumber,
                              OrgImage = u.OrgImage,
                              OrgAuthStatus = u.OrgAuthStatus,
                              OrgAuthMessage = u.OrgAuthMessage
                          })
                          .FirstOrDefaultAsync();

            return result;
        }
    }
}
