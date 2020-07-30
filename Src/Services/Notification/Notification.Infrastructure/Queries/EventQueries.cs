using Microsoft.Extensions.Logging;
using Photography.Services.Notification.API.Query.Interfaces;
using Photography.Services.Notification.API.Query.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Arise.DDD.API.Paging;
using Photography.Services.Notification.Domain.AggregatesModel.EventAggregate;

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
                                   CircleName = e.CircleName,
                                   OrderId = e.OrderId,
                                   Processed = e.Processed,
                                   Readed = e.Readed,
                                   Id = e.Id
                               };

            return await PagedList<EventViewModel>.ToPagedListAsync(queryableDto, pagingParameters);
        }

        public async Task<PagedList<EventViewModel>> GetUserCategoryEventsAsync(EventCategory eventCategory, PagingParameters pagingParameters)
        {
            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var eventTypes = EventCategoryTypeHelper.GetEventCategoryTypes(eventCategory);

            var queryableDto = from e in _dbContext.Events
                               join u in _dbContext.Users
                               on e.FromUserId equals u.Id
                               where e.ToUserId == myId && eventTypes.Contains(e.EventType)
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
                                   CircleName = e.CircleName,
                                   OrderId = e.OrderId,
                                   Processed = e.Processed,
                                   Readed = e.Readed,
                                   Id = e.Id
                               };

            return await PagedList<EventViewModel>.ToPagedListAsync(queryableDto, pagingParameters);
        }

        public async Task<UnReadEventCountViewModel> GetUnReadEventCountAsync()
        {
            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var sqlBuilder = new StringBuilder($"select {GetCountSql(EventCategory.Interaction)} as Interaction, ");
            sqlBuilder.Append($"{GetCountSql(EventCategory.Appointment)} as Appointment, ");
            sqlBuilder.Append($"{GetCountSql(EventCategory.System)} as System ");
            sqlBuilder.Append($"from Events where Readed = 0 and ToUserId = '{myId}'");

            return await _dbContext.UnReadEventCounts.FromSqlRaw(sqlBuilder.ToString()).FirstOrDefaultAsync();
        }

        private string GetCountSql(EventCategory eventCategory)
        {
            return $"isnull(sum(case when EventType in {GetEventCategorySql(eventCategory)} then 1 else 0 end), 0)";
        }

        private string GetEventCategorySql(EventCategory eventCategory)
        {
            var sqlBuilder = new StringBuilder("(");
            var i = 0;
            var eventTypes = EventCategoryTypeHelper.GetEventCategoryTypes(eventCategory);

            eventTypes.ForEach(e =>
            {
                sqlBuilder.Append((int)e);
                if (i < eventTypes.Count - 1)
                    sqlBuilder.Append(", ");
                i++;
            });

            sqlBuilder.Append(")");

            return sqlBuilder.ToString();
        }
    }
}
