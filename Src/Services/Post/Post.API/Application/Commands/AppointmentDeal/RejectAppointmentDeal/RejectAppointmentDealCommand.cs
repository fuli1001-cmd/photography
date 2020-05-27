using MediatR;
using Photography.Services.Post.API.Query.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.AppointmentDeal.RejectAppointmentDeal
{
    public class RejectAppointmentDealCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }

        // 要拒绝的约拍交易Id
        public Guid DealId { get; set; }
    }
}
