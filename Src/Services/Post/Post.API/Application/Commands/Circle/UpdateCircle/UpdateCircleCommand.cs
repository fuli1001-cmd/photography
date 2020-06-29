using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.Circle.UpdateCircle
{
    public class UpdateCircleCommand : IRequest<bool>
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
