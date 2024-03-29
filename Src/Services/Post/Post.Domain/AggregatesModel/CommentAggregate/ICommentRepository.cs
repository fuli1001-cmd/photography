﻿using Arise.DDD.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Post.Domain.AggregatesModel.CommentAggregate
{
    public interface ICommentRepository : IRepository<Comment>
    {
        Task<Comment> GetCommentAsync(Guid commentId);
    }
}
