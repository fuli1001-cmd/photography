﻿using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Photography.Services.Order.API.Application.Commands.SelectOriginal
{
    [DataContract]
    public class SelectOriginalCommand : IRequest<bool>
    {
        /// <summary>
        /// 上传原片的订单ID
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
