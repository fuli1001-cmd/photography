using MediatR;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.Post.ExaminePost
{
    /// <summary>
    /// 审核帖子
    /// </summary>
    public class ExaminePostCommand : IRequest<bool>
    {
        /// <summary>
        /// 要审核的帖子id
        /// </summary>
        public Guid PostId { get; set; }

        /// <summary>
        /// 审核状态
        /// </summary>
        public PostAuthStatus PostAuthStatus { get; set; }
    }
}
