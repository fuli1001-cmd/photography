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
    public class CancelAppointmentDealCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }

        public Guid DealId { get; set; }
    }
}
