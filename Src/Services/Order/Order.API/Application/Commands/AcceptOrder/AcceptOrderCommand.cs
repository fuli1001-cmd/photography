using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Order.API.Application.Commands.AcceptOrder
{
    public class AcceptOrderCommand : IRequest<bool>
    {
        public Guid DealId { get; set; }
    }
}
