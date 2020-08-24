using Arise.DDD.API.Paging;
using Photography.Services.User.API.Query.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Query.Interfaces
{
    public interface IUserQueries
    {
        /// <summary>
        /// 获取当前用户详情
        /// </summary>
        /// <returns></returns>
        Task<UserViewModel> GetCurrentUserAsync();

        /// <summary>
        /// 根据用户id、老系统的用户id、昵称三者之一获取用户详情
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="oldUserId">老系统用户id</param>
        /// <param name="nickName">昵称</param>
        /// <returns></returns>
        Task<UserViewModel> GetUserAsync(Guid? userId, int? oldUserId, string nickName);

        /// <summary>
        /// 获取当前用户的朋友
        /// </summary>
        /// <returns></returns>
        Task<List<FriendViewModel>> GetFriendsAsync();

        /// <summary>
        /// 获取当前用户的关注者
        /// </summary>
        /// <returns></returns>
        Task<PagedList<FollowerViewModel>> GetFollowersAsync(Guid userId, PagingParameters pagingParameters);

        /// <summary>
        /// 获取关注当前用户的人
        /// </summary>
        /// <returns></returns>
        Task<PagedList<FollowerViewModel>> GetFollowedUsersAsync(Guid userId, PagingParameters pagingParameters);

        /// <summary>
        /// 搜索用户
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<List<UserSearchResult>> SearchUsersAsync(string key);

        Task<PagedList<ExaminingUserViewModel>> GetExaminingUsersAsync(string key, PagingParameters pagingParameters);

        // 获取用户的团体认证信息
        Task<UserOrgAuthInfoViewModel> GetUserOrgAuthInfoAsync(Guid? userId = null);
    }
}
