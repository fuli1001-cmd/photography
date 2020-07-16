using MediatR;
using Photography.Services.Order.API.Query.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Order.API.Application.Commands.RejectOrder
{
    public class RejectOrderCommand : IRequest<OrderViewModel>
    {
        /// <summary>
        /// 要拒绝的订单所对应的约拍交易id
        /// </summary>
        public Guid DealId { get; set; }

        /// <summary>
        /// 拒绝说明
        /// </summary>
        public string Description { get; set; }
    }
}
