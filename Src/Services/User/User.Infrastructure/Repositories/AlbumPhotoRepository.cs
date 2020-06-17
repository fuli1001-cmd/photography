using Arise.DDD.Infrastructure;
using Photography.Services.User.Domain.AggregatesModel.AlbumPhotoAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.User.Infrastructure.Repositories
{
    public class AlbumPhotoRepository : EfRepository<AlbumPhoto, UserContext>, IAlbumPhotoRepository
    {
        public AlbumPhotoRepository(UserContext context) : base(context)
        {

        }
    }
}
