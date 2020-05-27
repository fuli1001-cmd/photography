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

namespace Photography.Services.Order.API.Application.Commands.UploadOriginal
{
    public class UploadOriginalCommandHandler : IRequestHandler<UploadOriginalCommand, OrderViewModel>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<UploadOriginalCommandHandler> _logger;
        private readonly IOrderQueries _orderQueries;

        public UploadOriginalCommandHandler(IOrderQueries orderQueries, 
            IOrderRepository orderRepository,
            ILogger<UploadOriginalCommandHandler> logger)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _orderQueries = orderQueries ?? throw new ArgumentNullException(nameof(orderQueries));
        }

        public async Task<OrderViewModel> Handle(UploadOriginalCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetOrderWithAttachmentsAsync(request.OrderId);
            order.UploadOriginalFiles(request.Attachments);
            await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
            return await _orderQueries.GetOrderAsync(order.Id);
        }
    }
}
