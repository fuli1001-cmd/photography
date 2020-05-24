using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Photography.Services.Order.API.Application.Commands.CheckProcessed
{
    [DataContract]
    public class CheckProcessedCommand : IRequest<bool>
    {
        /// <summary>
        /// 验收精修片的订单ID
        /// </summary>
        [DataMember]
        [Required]
        public Guid OrderId { get; set; }
    }
}
