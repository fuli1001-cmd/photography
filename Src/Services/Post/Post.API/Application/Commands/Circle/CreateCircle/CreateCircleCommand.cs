using MediatR;
using Photography.Services.Post.API.Query.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.Circle.CreateCircle
{
    /// <summary>
    /// 创建圈子的命令
    /// </summary>
    public class CreateCircleCommand : IRequest<CircleViewModel>
    {
        /// <summary>
        /// 圈子名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 圈子描述
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
