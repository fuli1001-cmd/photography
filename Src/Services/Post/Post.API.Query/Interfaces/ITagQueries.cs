using Photography.Services.Post.Domain.AggregatesModel.TagAggregate;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Query.Interfaces
{
    public interface ITagQueries
    {
        // 常用公共标签
        Task<IEnumerable<string>> GetPopularPublicTagsAsync();

        // 我的私有标签（帖子类别）
        Task<IEnumerable<string>> GetUserPrivateTagsAsync(Guid userId);

        // 系统标签
        Task<IEnumerable<string>> GetSystemTagsAsync();
    }
}
