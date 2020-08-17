using Photography.Services.User.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.User.API.Query.ViewModels
{
    public class UserViewModel : BaseUserViewModel
    {
        public string Avatar { get; set; }
        public UserType? UserType { get; set; }
        public string UserName { get; set; }
        public Gender? Gender { get; set; }
        public double? Birthday { get; set; }
        public int? Age { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Sign { get; set; }
        public int LikedCount { get; set; }
        public int FollowingCount { get; set; }
        public int FollowerCount { get; set; }

        public string BackgroundImage { get; set; }

        // 进行中的订单数量（除去已完成、已拒绝和已取消外的订单数量）【已废弃】
        public int OngoingOrderCount { get; set; }

        //// 拍片阶段的订单数量，包含OrderStatus为WaitingForShooting，WaitingForUploadOriginal的订单
        //public int ShootingStageOrderCount { get; set; }

        //// 选片阶段的订单数量，包含OrderStatus为WaitingForSelection的订单
        //public int SelectionStageOrderCount { get; set; }

        //// 出片阶段的订单数量，包含OrderStatus为WaitingForUploadProcessed，WaitingForCheck的订单
        //public int ProductionStageOrderCount { get; set; }

        // 发布的帖子数量
        public int PostCount { get; set; }

        // 点赞过的帖子的数量
        public int LikedPostCount { get; set; }

        // 发布的约拍数量
        public int AppointmentCount { get; set; }

        // 约拍值
        public int Score { get; set; }
        public string Phonenumber { get; set; }
        public bool Followed { get; set; }

        public bool ViewFollowersAllowed { get; set; }

        public bool ViewFollowedUsersAllowed { get; set; }

        public int ChatServerUserId { get; set; }

        public IdAuthStatus RealNameRegistrationStatus { get; set; }

        // 社团认证状态
        public IdAuthStatus OrgAuthStatus { get; set; }

        public string Code { get; set; }
    }

    public class FollowerViewModel : BaseUserViewModel
    {
        public string Avatar { get; set; }

        /// <summary>
        /// 是否已关注此人
        /// </summary>
        public bool Followed { get; set; }

        /// <summary>
        /// 粉丝数量
        /// </summary>
        public int FollowersCount { get; set; }

        /// <summary>
        /// 贴子数量
        /// </summary>
        public int PostCount { get; set; }
    }

    public class UserSearchResult : FollowerViewModel
    {
        public int PostCount { get; set; }
        public int FollowerCount { get; set; }
    }

    public class GroupUserViewModel : BaseUserViewModel
    {
        public string Avatar { get; set; }
        public int ChatServerUserId { get; set; }
    }

    public class ExaminingUserViewModel : BaseUserViewModel
    {
        public string Avatar { get; set; }

        public string Sign { get; set; }

        public string BackgroundImage { get; set; }

        public Gender? Gender { get; set; }

        public double? Birthday { get; set; }

        public UserType? UserType { get; set; }

        public string Province { get; set; }

        public string City { get; set; }

        public string IdCardFront { get; set; }

        public string IdCardBack { get; set; }

        public string IdCardHold { get; set; }

        public bool Disabled { get; set; }

        public IdAuthStatus RealNameRegistrationStatus { get; set; }

        // 社团认证状态
        public IdAuthStatus OrgAuthStatus { get; set; }
    }

    public class BaseUserViewModel
    {
        public Guid Id { get; set; }
        public string Nickname { get; set; }
    }
}
