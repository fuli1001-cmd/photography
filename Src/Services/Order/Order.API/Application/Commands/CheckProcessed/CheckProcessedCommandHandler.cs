using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Photography.Services.Order.API.Query.Interfaces;
using Photography.Services.Order.API.Query.ViewModels;
using Photography.Services.Order.Domain.AggregatesModel.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Order.API.Application.Commands.CheckProcessed
{
    public class CheckProcessedCommandHandler : IRequestHandler<CheckProcessedCommand, OrderViewModel>
    {
        private readonly IOrderQueries _orderQueries;
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<CheckProcessedCommandHandler> _logger;

        public CheckProcessedCommandHandler(IOrderQueries orderQueries, IOrderRepository postRepository, ILogger<CheckProcessedCommandHandler> logger)
        {
            _orderQueries = orderQueries ?? throw new ArgumentNullException(nameof(orderQueries));
            _orderRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OrderViewModel> Handle(CheckProcessedCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(request.OrderId);
            order.AcceptProcessedFiles();
            await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
            return await _orderQueries.GetOrderAsync(order.Id);
        }
    }
}
