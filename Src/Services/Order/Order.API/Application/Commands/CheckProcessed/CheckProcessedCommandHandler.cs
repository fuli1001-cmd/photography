using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Photography.Services.Order.Domain.AggregatesModel.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Order.API.Application.Commands.CheckProcessed
{
    public class CheckProcessedCommandHandler : IRequestHandler<CheckProcessedCommand, bool>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<CheckProcessedCommandHandler> _logger;

        public CheckProcessedCommandHandler(IOrderRepository postRepository, ILogger<CheckProcessedCommandHandler> logger)
        {
            _orderRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(CheckProcessedCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(request.OrderId);
            order.AcceptProcessedFiles();
            return await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
