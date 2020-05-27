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
    public class AcceptAppointmentDealCommand : IRequest<bool>
    {
        public Guid UserId;
        public Guid DealId { get; set; }
    }
}
