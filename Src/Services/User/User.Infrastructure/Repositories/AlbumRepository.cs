using Arise.DDD.Infrastructure;
using Photography.Services.User.Domain.AggregatesModel.AlbumAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Photography.Services.User.Infrastructure.Repositories
{
    public class AlbumRepository : EfRepository<Album, UserContext>, IAlbumRepository
    {
        public AlbumRepository(UserContext context) : base(context)
        {

        }

        public async Task<Album> GetAlbumWithPhotosAsync(Guid albumId)
        {
            return await _context.Albums.Where(a => a.Id == albumId).Include(a => a.AlbumPhotos).SingleOrDefaultAsync();
        }
    }
}
