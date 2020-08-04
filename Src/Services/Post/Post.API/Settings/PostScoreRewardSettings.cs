using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Settings
{
    public class PostScoreRewardSettings
    {
        // 用户发帖的初始分
        public int InitUser { get; set; }

        // 用户修改属性后加分
        public int SetUserProperty { get; set; }

        // 浏览帖子加分
        public int ViewPost { get; set; }

        // 赞帖加分
        public int LikePost { get; set; }

        // 评论帖加分
        public int CommentPost { get; set; }

        // 分享贴加分
        public int SharePost { get; set; }

        // 转发贴加分
        public int ForwardPost { get; set; }

        // 新用户发帖加分
        public int NewUserPost { get; set; }

        // 新用户时长（小时）
        public int NewUserHour { get; set; }

        // 自帖子发布起多少小时后开始刷新积分
        public int StartRefreshHour { get; set; }

        // 每个多少小时刷新积分一次
        public int RefreshIntervalHour { get; set; }

        // 每次刷新后，新积分为现积分的百分比
        public double Percent { get; set; }
    }
}
