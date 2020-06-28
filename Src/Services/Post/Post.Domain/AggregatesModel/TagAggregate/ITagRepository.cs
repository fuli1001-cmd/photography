using Arise.DDD.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Post.Domain.AggregatesModel.TagAggregate
{
    public interface ITagRepository : IRepository<Tag>
    {
        Task<List<Tag>> GetPublicTagsByNames(List<string> names);

        Task<Tag> GetUserPrivateTagByName(Guid userId, string name);
    }
}
