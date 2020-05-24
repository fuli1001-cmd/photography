using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.AppointmentDeal.ConfirmAppointmentDeal
{
    [DataContract]
    public class ConfirmAppointmentDealCommand : IRequest<bool>
    {
        /// <summary>
        /// 要确认的约拍交易Id
        /// </summary>
        [DataMember]
        [Required]
        public Guid AppointmentId { get; set; }
    }
}
