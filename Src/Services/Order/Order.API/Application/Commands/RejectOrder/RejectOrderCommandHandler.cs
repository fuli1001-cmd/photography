using Arise.DDD.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using Photography.Services.Order.Domain.AggregatesModel.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Order.API.Application.Commands.RejectOrder
{
    public class RejectOrderCommandHandler : IRequestHandler<RejectOrderCommand, bool>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<RejectOrderCommandHandler> _logger;

        public RejectOrderCommandHandler(IOrderRepository postRepository, ILogger<RejectOrderCommandHandler> logger)
        {
            _orderRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(RejectOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetbyDealIdAsync(request.DealId);
            if (order == null)
                throw new DomainException("没有与当前约拍交易对应的订单。");
            order.Reject();
            return await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
