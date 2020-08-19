using MediatR;
using Photography.Services.Order.Domain.AggregatesModel.OrderAggregate;
using Photography.Services.Order.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Photography.Services.Order.API.Application.Commands.CreateOrder
{
    public class CreateOrderCommand : IRequest<bool>
    {
        public Guid User1Id { get; set; }
        public Guid User2Id { get; set; }
        public UserType AppointmentedUserType { get; set; }
        public int PayerType { get; set; }
        public Guid DealId { get; set; }
        public decimal Price { get; set; }
        public double AppointedTime { get; set; }
        public Guid? PayerId { get; set; }
        public string Text { get; set; }

        #region location properties
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string LocationName { get; set; }

        public string Address { get; set; }
        #endregion
    }
}
