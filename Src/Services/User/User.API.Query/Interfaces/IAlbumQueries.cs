using Arise.DDD.API.Paging;
using Photography.Services.User.API.Query.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Query.Interfaces
{
    public interface IAlbumQueries
    {
        Task<IEnumerable<AlbumViewModel>> GetAlbumsAsync();

        Task<PagedList<AlbumViewModel>> GetAlbumsAsync(PagingParameters pagingParameters, string orderBy, bool asc);

        Task<AlbumViewModel> GetAlbumAsync(Guid albumId);

        Task<PagedList<AlbumPhotoViewModel>> GetAlbumPhotosAsync(Guid albumId, PagingParameters pagingParameters, string orderBy, bool asc);
    }
}
