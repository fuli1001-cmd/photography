using Arise.DDD.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Post.Domain.AggregatesModel.PostAggregate
{
    public interface IPostRepository : IRepository<Post>
    {
        Task LoadUserAsync(Post post);

        Task<Post> GetPostWithAppointmentedUserById(Guid postId);

        Task<Post> GetPostWithAttachmentsById(Guid postId);

        Task<Post> GetPostWithNavigationPropertiesById(Guid postId);

        Task<Post> GetAppointmentById(Guid postId);

        Task<int> GetPostCommentCountAsync(Guid postId);

        //Task<Dictionary<Guid, Guid>> GetPostsUserIdsAsync(List<Guid> postIds);

        Task<List<Post>> GetPostsAsync(List<Guid> postIds);

        Task<List<Post>> GetUserPostsByPrivateTag(Guid userId, string privateTag);

        Task<List<Post>> GetUserPostsAsync(Guid userId);

        /// <summary>
        /// 刷新帖子积分
        /// 规则：从发布后第startRefreshHour小时起，积分每refreshIntervalHour小时衰减为现积分的percent
        /// </summary>
        /// <param name="startRefreshHour">自帖子发布多少小时侯开始刷新帖子积分</param>
        /// <param name="refreshIntervalHour">每隔多少小时刷新一次</param>
        /// <param name="percent">衰减为现积分的percent</param>
        /// <returns></returns>
        Task RefreshPostScore(int startRefreshHour, int refreshIntervalHour, double percent);


        /// <summary>
        /// 获取用户今天已发出的约拍交易数量
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<int> GetTodayUserSentAppointmentDealCountAsync(Guid userId);

        /// <summary>
        /// 获取用户今天已收到的约拍交易数量
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<int> GetTodayUserReceivedAppointmentDealCountAsync(Guid userId);
    }
}
