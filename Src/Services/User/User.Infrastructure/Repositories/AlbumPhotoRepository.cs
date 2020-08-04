using Arise.DDD.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Photography.Services.User.Domain.AggregatesModel.AlbumPhotoAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.User.Infrastructure.Repositories
{
    public class AlbumPhotoRepository : EfRepository<AlbumPhoto, UserContext>, IAlbumPhotoRepository
    {
        public AlbumPhotoRepository(UserContext context) : base(context)
        {

        }

        public async Task<List<AlbumPhoto>> GetPhotosAsync(IEnumerable<Guid> photoIds)
        {
            return await _context.AlbumPhotos.Where(p => photoIds.Contains(p.Id)).ToListAsync();
        }
    }
}
