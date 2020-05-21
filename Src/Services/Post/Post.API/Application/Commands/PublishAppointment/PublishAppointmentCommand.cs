using MediatR;
using Photography.Services.Post.API.Application.Commands.PublishPost;
using Photography.Services.Post.API.Query.ViewModels;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.PublishAppointment
{
    [DataContract]
    public class PublishAppointmentCommand : IRequest<AppointmentViewModel>
    {
        /// <summary>
        /// 约拍描述
        /// </summary>
        [DataMember]
        public string Text { get; set; }

        /// <summary>
        /// 约拍时间
        /// </summary>
        [DataMember]
        public double AppointedTime { get; set; }

        /// <summary>
        /// 约拍价格
        /// </summary>
        [DataMember]
        public decimal Price { get; set; }

        /// <summary>
        /// 付费方类型
        /// </summary>
        [DataMember]
        public PayerType PayerType { get; set; }

        /// <summary>
        /// 附件数组
        /// </summary>
        [DataMember]
        [Required]
        public List<Attachment> attachments { get; set; }

        /// <summary>
        /// 城市代码
        /// </summary>
        [DataMember]
        public string CityCode { get; set; }

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
    }
}
