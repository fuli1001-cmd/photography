using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.API.Query.Interfaces;
using Photography.Services.Post.API.Query.ViewModels;
using Photography.Services.Post.Infrastructure.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Query.EF
{
    public class UserQueries : IUserQueries
    {
        private readonly PostContext _postContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly ILogger<UserQueries> _logger;

        public UserQueries(PostContext postContext, IHttpContextAccessor httpContextAccessor, IMapper mapper, ILogger<UserQueries> logger)
        {
            _postContext = postContext ?? throw new ArgumentNullException(nameof(postContext));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public UserViewModel GetCurrentUserAsync()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = _postContext.Users.SingleOrDefault(u => u.Id.ToString() == userId);
            return _mapper.Map<UserViewModel>(user);
        }

        public List<FriendViewModel> GetFriendsAsync()
        {
            _logger.LogInformation("*********GetFriendsAsync**********");
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            _logger.LogInformation("*********{id}**********", userId);
            var friendsIds = GetFriendsIds(userId);
            _logger.LogInformation("*********printing 1**********");
            //await friendsIds.ForEachAsync(f => _logger.LogInformation("*****************friends: {Friend}*****************", f));
            var friends = _postContext.Users
                .Where(u => friendsIds.Contains(u.Id));
            _logger.LogInformation("*********printing 2**********");
            //friends.ForEach(f => _logger.LogInformation(f.Nickname));
            return _mapper.Map<List<FriendViewModel>>(friends);
        }

        public IQueryable<Guid> GetFriendsIds(string userId)
        {
            var friendsQuery = from ur1 in _postContext.UserRelations
                               join ur2 in _postContext.UserRelations on
                               new { C1 = ur1.FollowerId, C2 = ur1.FollowedUserId }
                               equals
                               new { C1 = ur2.FollowedUserId, C2 = ur2.FollowerId }
                               where ur1.FollowerId.ToString() == userId
                               select ur1.FollowedUserId;

            return friendsQuery;
        }
    }
}
