using MediatR;
using Photography.Services.Post.API.Query.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.AppointmentDeal.AcceptAppointmentDeal
{
    [DataContract]
    public class AcceptAppointmentDealCommand : IRequest<AppointmentViewModel>
    {
        /// <summary>
        /// 要接受的约拍交易Id
        /// </summary>
        [DataMember]
        [Required]
        public Guid AppointmentId { get; set; }
    }
}
