using MediatR;
using Photography.Services.Order.API.Query.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Photography.Services.Order.API.Application.Commands.UploadProcessed
{
    [DataContract]
    public class UploadProcessedCommand : IRequest<OrderViewModel>
    {
        /// <summary>
        /// 上传精修片的订单ID
        /// </summary>
        [DataMember]
        [Required]
        public Guid OrderId { get; set; }

        /// <summary>
        /// 附件名称数组
        /// </summary>
        public List<string> Attachments { get; set; }
    }
}
