using Arise.DDD.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Post.Domain.AggregatesModel.UserCommentRelationAggregate
{
    public interface IUserCommentRelationRepository : IRepository<UserCommentRelation>
    {
        Task<UserCommentRelation> GetAsync(Guid userId, Guid commentId);
    }
}
