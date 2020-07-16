using MediatR;
using Photography.Services.Order.API.Query.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Order.API.Application.Commands.CancelOrder
{
    public class CancelOrderCommand : IRequest<OrderViewModel>
    {
        /// <summary>
        /// 要取消的订单所对应的约拍交易id
        /// </summary>
        public Guid DealId { get; set; }

        /// <summary>
        /// 取消说明
        /// </summary>
        public string Description { get; set; }
    }
}
