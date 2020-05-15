﻿using Photography.Services.Post.Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Post.Domain.AggregatesModel.UserPostRelationAggregate
{
    public interface IUserPostRelationRepository : IRepository<UserPostRelation>
    {
        Task<UserPostRelation> GetAsync(Guid userId, Guid postId);
    }
}
