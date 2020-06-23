using Arise.DDD.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.User.Domain.AggregatesModel.AlbumPhotoAggregate
{
    public interface IAlbumPhotoRepository : IRepository<AlbumPhoto>
    {
        Task<List<AlbumPhoto>> GetPhotosAsync(IEnumerable<Guid> photoIds);
    }
}
