using Arise.DDD.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Post.Domain.AggregatesModel.PostAggregate
{
    public interface IPostRepository : IRepository<Post>
    {
        void LoadUser(Post post);

        Task<Post> GetPostWithAppointmentedUserById(Guid postId);

        Task<Post> GetPostWithAttachmentsById(Guid postId);

        Task<Post> GetPostWithNavigationPropertiesById(Guid postId);

        Task<Post> GetAppointmentById(Guid postId);

        Task<int> GetPostCommentCountAsync(Guid postId);

        //Task<Dictionary<Guid, Guid>> GetPostsUserIdsAsync(List<Guid> postIds);

        Task<List<Post>> GetPostsAsync(List<Guid> postIds);

        Task<List<Post>> GetUserPostsByPrivateTag(Guid userId, string privateTag);

        Task<List<Post>> GetUserPostsAsync(Guid userId);
    }
}
