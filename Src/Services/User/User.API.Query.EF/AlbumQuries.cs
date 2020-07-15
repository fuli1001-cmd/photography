using Arise.DDD.API.Paging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Photography.Services.User.API.Query.Interfaces;
using Photography.Services.User.API.Query.ViewModels;
using Photography.Services.User.Infrastructure;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Photography.Services.User.API.Query.EF
{
    public class AlbumQuries : IAlbumQueries
    {
        private readonly UserContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AlbumQuries> _logger;

        public AlbumQuries(UserContext dbContext,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AlbumQuries> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region 相册
        public async Task<AlbumViewModel> GetAlbumAsync(Guid albumId)
        {
            var queryableAlbums = _dbContext.Albums.Where(a => a.Id == albumId);
            return await GetQueryableAlbumViewModels(queryableAlbums).SingleOrDefaultAsync();
        }

        public async Task<PagedList<AlbumViewModel>> GetAlbumsAsync(PagingParameters pagingParameters, string orderBy, bool asc)
        {
            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var queryableAlbums = from a in _dbContext.Albums
                                  where a.UserId == myId
                                  select a;

            if (string.Compare(orderBy, "CreatedTime", true) == 0)
                queryableAlbums = asc ? queryableAlbums.OrderBy(a => a.CreatedTime) : queryableAlbums.OrderByDescending(a => a.CreatedTime);
            else
                queryableAlbums = asc ? queryableAlbums.OrderBy(a => a.UpdatedTime) : queryableAlbums.OrderByDescending(a => a.UpdatedTime);

            var queryableAblumViewModels = GetQueryableAlbumViewModels(queryableAlbums);

            return await PagedList<AlbumViewModel>.ToPagedListAsync(queryableAblumViewModels, pagingParameters);
        }

        public async Task<IEnumerable<AlbumViewModel>> GetAlbumsAsync()
        {
            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var queryableAlbums = from a in _dbContext.Albums
                                  where a.UserId == myId
                                  orderby a.Name
                                  select a;

            return await GetQueryableAlbumViewModels(queryableAlbums).ToListAsync();
        }

        private IQueryable<AlbumViewModel> GetQueryableAlbumViewModels(IQueryable<Domain.AggregatesModel.AlbumAggregate.Album> queryableAlbums)
        {
            return from a in queryableAlbums
                   select new AlbumViewModel
                   {
                       Id = a.Id,
                       Name = a.Name,
                       CoverPhoto = a.AlbumPhotos.OrderByDescending(p => p.UpdatedTime).FirstOrDefault().Name,
                       PhotoCount = a.AlbumPhotos.Count()
                   };
        }
        #endregion

        #region 相册照片
        public async Task<PagedList<AlbumPhotoViewModel>> GetAlbumPhotosAsync(Guid albumId, PagingParameters pagingParameters, string orderBy, bool asc)
        {
            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var queryablePhotos = from a in _dbContext.Albums
                                  join ap in _dbContext.AlbumPhotos
                                  on a.Id equals ap.AlbumId
                                  where a.Id == albumId && a.UserId == myId
                                  select ap;

            if (string.Compare(orderBy, "CreatedTime", true) == 0)
                queryablePhotos = asc ? queryablePhotos.OrderBy(ap => ap.CreatedTime) : queryablePhotos.OrderByDescending(ap => ap.CreatedTime);
            else
                queryablePhotos = asc ? queryablePhotos.OrderBy(ap => ap.UpdatedTime) : queryablePhotos.OrderByDescending(ap => ap.UpdatedTime);

            var queryablePhotoViewModels = queryablePhotos.Select(ap => new AlbumPhotoViewModel 
            {
                Id = ap.Id,
                Name = ap.Name,
                DisplayName = ap.DisplayName,
                CreatedTime = ap.CreatedTime,
                UpdatedTime = ap.UpdatedTime
            });

            return await PagedList<AlbumPhotoViewModel>.ToPagedListAsync(queryablePhotoViewModels, pagingParameters);
        }
        #endregion
    }
}
