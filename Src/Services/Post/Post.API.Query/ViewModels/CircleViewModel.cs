using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.API.Query.ViewModels
{
    public class CircleViewModel
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
        public string BackgroundImage { get; set; }
    }
}
