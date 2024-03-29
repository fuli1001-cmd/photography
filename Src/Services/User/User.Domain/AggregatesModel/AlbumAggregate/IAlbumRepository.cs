﻿using Arise.DDD.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.User.Domain.AggregatesModel.AlbumAggregate
{
    public interface IAlbumRepository : IRepository<Album>
    {
        Task<Album> GetAlbumWithPhotosAsync(Guid albumId);
    }
}
