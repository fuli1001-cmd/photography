using MediatR;
using Photography.Services.Order.API.Query.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Order.API.Application.Commands.AcceptOrder
{
    public class AcceptOrderCommand : IRequest<OrderViewModel>
    {
        public Guid DealId { get; set; }
    }
}
