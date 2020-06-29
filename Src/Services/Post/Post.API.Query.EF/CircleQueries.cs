using Arise.DDD.API.Paging;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.API.Query.Interfaces;
using Photography.Services.Post.API.Query.ViewModels;
using Photography.Services.Post.Domain.AggregatesModel.CircleAggregate;
using Photography.Services.Post.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Query.EF
{
    public class CircleQueries : ICircleQueries
    {
        private readonly PostContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CircleQueries> _logger;

        public CircleQueries(PostContext postContext, IHttpContextAccessor httpContextAccessor, ILogger<CircleQueries> logger)
        {
            _dbContext = postContext ?? throw new ArgumentNullException(nameof(postContext));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<CircleViewModel> GetCircleAsync(Guid circleId)
        {
            var queryableCircle = _dbContext.Circles.Where(c => c.Id == circleId);
            return await GetCircleViewModel(queryableCircle).SingleOrDefaultAsync();
        }

        public async Task<PagedList<CircleViewModel>> GetCirclesAsync(PagingParameters pagingParameters)
        {
            var queryableCircle = _dbContext.Circles.OrderByDescending(c => c.UserCount);
            var queryableDto = GetCircleViewModel(queryableCircle);
            return await PagedList<CircleViewModel>.ToPagedListAsync(queryableDto, pagingParameters);
        }

        public async Task<PagedList<CircleViewModel>> GetMyCirclesAsync(PagingParameters pagingParameters)
        {
            var claim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            var myId = claim == null ? Guid.Empty : Guid.Parse(claim.Value);

            var queryableCircle = from c in _dbContext.Circles
                                  join uc in _dbContext.UserCircleRelations
                                  on new { CircleId = c.Id, UserId = myId } equals new { CircleId = uc.CircleId, UserId = uc.UserId }
                                  orderby uc.Topping, uc.JoinTime descending
                                  select c;
            
            var queryableDto = GetCircleViewModel(queryableCircle);

            return await PagedList<CircleViewModel>.ToPagedListAsync(queryableDto, pagingParameters);
        }

        private IQueryable<CircleViewModel> GetCircleViewModel(IQueryable<Circle> queryableCircle)
        {
            return queryableCircle.Select(c => new CircleViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                VerifyJoin = c.VerifyJoin,
                BackgroundImage = c.BackgroundImage
            });
        }
    }
}
