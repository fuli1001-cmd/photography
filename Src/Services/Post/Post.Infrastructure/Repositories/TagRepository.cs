using Arise.DDD.Infrastructure;
using Photography.Services.Post.Domain.AggregatesModel.TagAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Infrastructure.Repositories
{
    public class TagRepository : EfRepository<Tag, PostContext>, ITagRepository
    {
        public TagRepository(PostContext context) : base(context)
        {

        }
    }
}
