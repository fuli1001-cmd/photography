using AutoMapper;
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

namespace Photography.Services.Order.API.Application.Commands.SelectOriginal
{
    public class SelectOriginalCommandHandler : IRequestHandler<SelectOriginalCommand, OrderViewModel>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderQueries _orderQueries;
        private readonly ILogger<SelectOriginalCommandHandler> _logger;

        public SelectOriginalCommandHandler(IOrderQueries orderQueries, 
            IOrderRepository orderRepository,
            ILogger<SelectOriginalCommandHandler> logger)
        {
            _orderQueries = orderQueries ?? throw new ArgumentNullException(nameof(orderQueries));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OrderViewModel> Handle(SelectOriginalCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetOrderWithAttachmentsAsync(request.OrderId);
            order.SelectOriginalFiles(request.Attachments);
            await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
            return await _orderQueries.GetOrderAsync(order.Id);
        }
    }
}
