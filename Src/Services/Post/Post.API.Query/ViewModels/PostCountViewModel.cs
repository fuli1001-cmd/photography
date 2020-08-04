using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.API.Query.ViewModels
{
    /// <summary>
    /// 3种帖子数量
    /// </summary>
    public class PostCountViewModel
    {
        /// <summary>
        /// 帖子数量
        /// </summary>
        public int PostCount { get; set; }

        /// <summary>
        /// 约拍数量
        /// </summary>
        public int AppointmentCount { get; set; }

        /// <summary>
        /// 点赞的帖子数量
        /// </summary>
        public int LikedPostCount { get; set; }
    }
}
