using MediatR;
using Photography.Services.Post.API.Application.Commands.Appointment.PublishAppointment;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.AppointmentDeal.AppointUser
{
    [DataContract]
    public class AppointUserCommand : PublishAppointmentCommand
    {
        /// <summary>
        /// 要约拍的人的id
        /// </summary>
        [DataMember]
        [Required]
        public Guid AppointmentedUserId { get; set; }
    }
}
