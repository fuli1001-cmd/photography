using Microsoft.Extensions.Logging;
using Photography.Services.Notification.API.Query.Interfaces;
using Photography.Services.Notification.API.Query.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Photography.Services.Notification.Domain.AggregatesModel.UserAggregate;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Arise.DDD.API.Paging;

namespace Photography.Services.Notification.Infrastructure.Queries
{
    public class EventQueries : IEventQueries
    {
        private readonly NotificationContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<EventQueries> _logger;

        public EventQueries(NotificationContext dbContext, IHttpContextAccessor httpContextAccessor, ILogger<EventQueries> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PagedList<EventViewModel>> GetUserReceivedEventsAsync(PagingParameters pagingParameters)
        {
            var claim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            var myId = claim == null ? Guid.Empty : Guid.Parse(claim.Value);

            var queryableDto = from e in _dbContext.Events
                               join u in _dbContext.Users
                               on e.FromUserId equals u.Id
                               where e.ToUserId == myId
                               orderby e.CreatedTime descending
                               select new EventViewModel
                               {
                                   FromUser = new UserViewModel
                                   {
                                       Id = u.Id,
                                       Nickname = u.Nickname,
                                       Avatar = u.Avatar,
                                       Followed = (from ur in _dbContext.UserRelations
                                                   where ur.FollowerId == myId && ur.FollowedUserId == u.Id
                                                   select ur.Id)
                                                  .Any()
                                   },
                                   EventType = e.EventType,
                                   Image = (from p in _dbContext.Posts
                                            where p.Id == e.PostId
                                            select p.Image)
                                           .SingleOrDefault(),
                                   CreatedTime = e.CreatedTime,
                                   PostId = e.PostId,
                                   CommentId = e.CommentId,
                                   CommentText = e.CommentText,
                                   CircleId = e.CircleId,
                                   CircleName = e.CircleName
                               };

            return await PagedList<EventViewModel>.ToPagedListAsync(queryableDto, pagingParameters);
        }
    }
}
