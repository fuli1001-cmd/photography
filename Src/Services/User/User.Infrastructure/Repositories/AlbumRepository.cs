using Arise.DDD.Infrastructure;
using Photography.Services.User.Domain.AggregatesModel.AlbumAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.User.Infrastructure.Repositories
{
    public class AlbumRepository : EfRepository<Album, UserContext>, IAlbumRepository
    {
        public AlbumRepository(UserContext context) : base(context)
        {

        }
    }
}
