using Arise.DDD.Infrastructure;
using Photography.Services.Post.Domain.AggregatesModel.CircleAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Infrastructure.Repositories
{
    public class CircleRepository : EfRepository<Circle, PostContext>, ICircleRepository
    {
        public CircleRepository(PostContext context) : base(context)
        {

        }
    }
}
