using Arise.DDD.API.Paging;
using Microsoft.Extensions.Logging;
using Photography.Services.User.API.Query.Interfaces;
using Photography.Services.User.API.Query.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Photography.Services.User.Infrastructure.Queries
{
    public class GroupQueries : IGroupQueries
    {
        private readonly UserContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<GroupQueries> _logger;

        public GroupQueries(UserContext dbContext,
            IHttpContextAccessor httpContextAccessor, 
            ILogger<GroupQueries> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<GroupViewModel> GetGroupAsync(Guid? groupId, int? oldGroupId)
        {
            IQueryable<Domain.AggregatesModel.GroupAggregate.Group> queryableGroups = null;

            if (groupId != null)
                queryableGroups = from g in _dbContext.Groups
                                  where g.Id == groupId.Value
                                  select g;
            else if (oldGroupId != null)
                queryableGroups = from g in _dbContext.Groups
                                  where g.ChatServerGroupId == oldGroupId.Value
                                  select g;
            else
                return null;

            return await GetQueryableGroupViewModels(queryableGroups).SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<GroupViewModel>> GetGroupsAsync()
        {
            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var queryableGroups = from g in _dbContext.Groups
                                  join gu in _dbContext.GroupUsers
                                  on g.Id equals gu.GroupId
                                  where gu.UserId == myId
                                  select g;

            return await GetQueryableGroupViewModels(queryableGroups).ToListAsync();
        }

        private IQueryable<GroupViewModel> GetQueryableGroupViewModels(IQueryable<Domain.AggregatesModel.GroupAggregate.Group> queryableGroups)
        {
            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            return from g in queryableGroups
                   select new GroupViewModel
                   {
                       Id = g.Id,
                       Name = g.Name,
                       Notice = g.Notice,
                       Avatar = g.Avatar,
                       OwnerId = g.OwnerId,
                       ChatServerGroupId = g.ChatServerGroupId,
                       ModifyMemberEnabled = g.ModifyMemberEnabled,
                       Muted = (from gu in g.GroupUsers
                                where gu.UserId == myId
                                select gu.Muted).SingleOrDefault(),
                       Members = from gu in g.GroupUsers
                                 join u in _dbContext.Users
                                 on gu.UserId equals u.Id
                                 select new GroupUserViewModel
                                 {
                                     Id = u.Id,
                                     Nickname = u.Nickname,
                                     Avatar = u.Avatar,
                                     ChatServerUserId = u.ChatServerUserId
                                 }
                   };
        }
    }
}
