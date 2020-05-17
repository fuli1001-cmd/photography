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

        public UserViewModel GetCurrentUserAsync()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = _identityContext.Users.SingleOrDefault(u => u.Id.ToString() == userId);
            return _mapper.Map<UserViewModel>(user);
        }

        public List<FriendViewModel> GetFriendsAsync()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var friendsIds = GetFriendsIds(userId);
            var friends = _identityContext.Users
                .Where(u => friendsIds.Contains(u.Id));
            return _mapper.Map<List<FriendViewModel>>(friends);
        }

        private IQueryable<Guid> GetFriendsIds(string userId)
        {
            var friendsQuery = from ur1 in _identityContext.UserRelations
                               join ur2 in _identityContext.UserRelations on
                               new { C1 = ur1.FollowerId, C2 = ur1.FollowedUserId }
                               equals
                               new { C1 = ur2.FollowedUserId, C2 = ur2.FollowerId }
                               where ur1.FollowerId.ToString() == userId
                               select ur1.FollowedUserId;

            return friendsQuery;
        }
    }
}
