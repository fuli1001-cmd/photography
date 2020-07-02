using Arise.DDD.Infrastructure;
using Photography.Services.Post.Domain.AggregatesModel.UserShareAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Infrastructure.Repositories
{
    public class UserShareRepository : EfRepository<UserShare, PostContext>, IUserShareRepository
    {
        public UserShareRepository(PostContext context) : base(context) { }
    }
}
