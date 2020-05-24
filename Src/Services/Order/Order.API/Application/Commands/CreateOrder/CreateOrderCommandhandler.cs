using MediatR;
using Microsoft.Extensions.Logging;
using Photography.Services.Order.Domain.AggregatesModel.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Order.API.Application.Commands.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, bool>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<CreateOrderCommandHandler> _logger;

        public CreateOrderCommandHandler(IOrderRepository postRepository, ILogger<CreateOrderCommandHandler> logger)
        {
            _orderRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = new Domain.AggregatesModel.OrderAggregate.Order(request.User1Id, request.User2Id,
                request.DealId, request.PayerId, request.Price, request.AppointedTime, request.Text, 
                request.Latitude, request.Longitude, request.LocationName, request.Address);
            _orderRepository.Add(order);
            return await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
