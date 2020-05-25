using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Order.API.Application.Commands.CancelOrder
{
    public class CancelOrderCommand : IRequest<bool>
    {
        public Guid DealId { get; set; }
    }
}
