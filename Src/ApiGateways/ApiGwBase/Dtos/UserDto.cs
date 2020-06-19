using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.ApiGateways.ApiGwBase.ViewModels
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Nickname { get; set; }
        public string Avatar { get; set; }
        public int? UserType { get; set; }
        public string UserName { get; set; }
        public int? Gender { get; set; }
        public double? Birthday { get; set; }
        public int? Age { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Sign { get; set; }
        public int LikedCount { get; set; }
        public int FollowingCount { get; set; }
        public int FollowerCount { get; set; }

        public string BackgroundImage { get; set; }

        // 进行中的订单数量（除去已完成、已拒绝和已取消外的订单数量）
        public int OngoingOrderCount { get; set; }

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

        public int PushLikeEvent { get; set; }
        public int PushReplyEvent { get; set; }
        public int PushForwardEvent { get; set; }
        public int PushShareEvent { get; set; }
        public int PushFollowEvent { get; set; }

        public int ChatServerUserId { get; set; }
    }
}
