using MediatR;
using Photography.Services.Post.API.Query.ViewModels;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.AppointmentDeal.CancelAppointmentDeal
{
    [DataContract]
    public class CancelAppointmentDealCommand : IRequest<bool>
    {
        /// <summary>
        /// 要取消的约拍交易Id
        /// </summary>
        [DataMember]
        [Required]
        public Guid AppointmentId { get; set; }
    }
}
