using MediatR;
using Photography.Services.Post.API.Query.ViewModels;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.PublishPost
{
    [DataContract]
    public class PublishPostCommand : IRequest<SameCityPostViewModel>
    {
        /// <summary>
        /// 文字描述
        /// </summary>
        [DataMember]
        public string Text { get; set; }

        /// <summary>
        /// 是否允许评论
        /// </summary>
        [DataMember]
        public bool Commentable { get; set; }

        /// <summary>
        /// 转发类型
        /// 0： 可转发
        /// 1：不可转发
        /// 2：仅好友可转发
        /// </summary>
        [DataMember]
        public ForwardType ForwardType { get; set; }

        /// <summary>
        /// 分享类型
        /// 0： 可分享
        /// 1：不可分享
        /// 2：仅好友可分享
        /// </summary>
        [DataMember]
        public ShareType ShareType { get; set; }

        /// <summary>
        /// 谁可以看
        /// 0：公开
        /// 1：仅好友
        /// 2：部分好友
        /// 3：密码查看
        /// </summary>
        [DataMember]
        public Visibility Visibility { get; set; }

        /// <summary>
        /// 查看密码
        /// </summary>
        [DataMember]
        public string ViewPassword { get; set; }

        /// <summary>
        /// 可查看帖子的好友id数组
        /// </summary>
        [DataMember]
        public List<Guid> friendIds { get; set; }

        /// <summary>
        /// 附件数组
        /// </summary>
        [DataMember]
        [Required]
        public List<Attachment> attachments { get; set; }

        /// <summary>
        /// 省
        /// </summary>
        [DataMember]
        public string Province { get; set; }

        /// <summary>
        /// 市
        /// </summary>
        [DataMember]
        public string City { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        [DataMember]
        public double Latitude { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        [DataMember]
        public double Longitude { get; set; }

        /// <summary>
        /// 地址名称
        /// </summary>
        [DataMember]
        public string LocationName { get; set; }

        /// <summary>
        /// 街道地址
        /// </summary>
        [DataMember]
        public string Address { get; set; }
    }

    /// <summary>
    /// 附件
    /// </summary>
    public class Attachment
    {
        /// <summary>
        /// 附件文件名
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// 附件描述
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        [Required]
        public string ContentType { get; set; }
    }
}
