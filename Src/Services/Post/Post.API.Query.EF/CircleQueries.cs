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

        /// <summary>
        /// 圈子详情
        /// </summary>
        /// <param name="circleId"></param>
        /// <returns></returns>
        public async Task<CircleViewModel> GetCircleAsync(Guid circleId)
        {
            var queryableCircle = _dbContext.Circles.Where(c => c.Id == circleId);
            var result = await GetCircleViewModel(queryableCircle).SingleOrDefaultAsync();
            if (result != null)
                SetImageProperties(result.BackgroundImage);
            return result;
        }

        /// <summary>
        /// 分页获取所有的圈子
        /// </summary>
        /// <param name="key"></param>
        /// <param name="pagingParameters"></param>
        /// <returns></returns>
        public async Task<PagedList<CircleViewModel>> GetCirclesAsync(string key, PagingParameters pagingParameters)
        {
            IQueryable<Circle> queryableCircle = _dbContext.Circles;

            if (!string.IsNullOrEmpty(key))
                queryableCircle = _dbContext.Circles.Where(c => c.Name.ToLower().Contains(key.ToLower()));

            queryableCircle = queryableCircle.OrderByDescending(c => c.UserCount).ThenBy(c => c.Name);

            var queryableDto = GetCircleViewModel(queryableCircle);
            var result = await PagedList<CircleViewModel>.ToPagedListAsync(queryableDto, pagingParameters);
            result.ForEach(c => SetImageProperties(c.BackgroundImage));
            return result;
        }

        /// <summary>
        /// 分页获取我的圈子
        /// </summary>
        /// <param name="pagingParameters"></param>
        /// <returns></returns>
        public async Task<PagedList<CircleViewModel>> GetMyCirclesAsync(PagingParameters pagingParameters)
        {
            var claim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            var myId = claim == null ? Guid.Empty : Guid.Parse(claim.Value);

            var queryableCircle = from c in _dbContext.Circles
                                  join uc in _dbContext.UserCircleRelations
                                  on new { CircleId = c.Id, UserId = myId } equals new { CircleId = uc.CircleId, UserId = uc.UserId }
                                  orderby uc.Topping descending, uc.JoinTime descending
                                  select c;
            
            var queryableDto = GetCircleViewModel(queryableCircle);

            var result = await PagedList<CircleViewModel>.ToPagedListAsync(queryableDto, pagingParameters);

            result.ForEach(c => SetImageProperties(c.BackgroundImage));

            return result;
        }

        private IQueryable<CircleViewModel> GetCircleViewModel(IQueryable<Circle> queryableCircle)
        {
            var claim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            var myId = claim == null ? Guid.Empty : Guid.Parse(claim.Value);

            return queryableCircle.Select(c => new CircleViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                VerifyJoin = c.VerifyJoin,
                BackgroundImage = new ImageViewModel { Name = c.BackgroundImage },
                OwnerId = c.OwnerId,
                UserCount = c.UserCount,
                IsInCircle = myId == Guid.Empty ? false : c.UserCircleRelations.Any(uc => uc.UserId == myId),
                Topping = myId == Guid.Empty ? false : (c.UserCircleRelations.Any(uc => uc.UserId == myId) ? c.UserCircleRelations.SingleOrDefault(uc => uc.UserId == myId).Topping : false)
            });
        }

        /// <summary>
        /// 设置图片宽高属性
        /// </summary>
        /// <param name="image"></param>
        private void SetImageProperties(ImageViewModel image)
        {
            if (!string.IsNullOrWhiteSpace(image.Name))
            {
                var sections = image.Name.Split('$');
                try
                {
                    image.Width = int.Parse(sections[1]);
                    image.Height = int.Parse(sections[2]);
                }
                catch
                {

                }
            }
        }
    }
}
