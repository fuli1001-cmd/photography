using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.API.Query.ViewModels
{
    /// <summary>
    /// 帖子需要包含的圈子信息
    /// </summary>
    public class PostCircleViewModel
    {
        /// <summary>
        /// 圈子ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 圈子名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 圈主id
        /// </summary>
        public Guid OwnerId { get; set; }
    }

    public class CircleViewModel : PostCircleViewModel
    {
        /// <summary>
        /// 圈子简介
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 入圈审核
        /// </summary>
        public bool VerifyJoin { get; set; }

        /// <summary>
        /// 圈子封面图
        /// </summary>
        public ImageViewModel BackgroundImage { get; set; }

        /// <summary>
        /// 圈子内人数
        /// </summary>
        public int UserCount { get; set; }

        /// <summary>
        /// 是否已加入圈子
        /// </summary>
        public bool IsInCircle { get; set; }

        /// <summary>
        /// 用户是否已置顶该圈子
        /// </summary>
        public bool Topping { get; set; }
    }

    public class ImageViewModel
    {
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
