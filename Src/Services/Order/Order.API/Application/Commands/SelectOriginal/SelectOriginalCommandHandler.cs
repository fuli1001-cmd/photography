using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Photography.Services.Order.Domain.AggregatesModel.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Order.API.Application.Commands.SelectOriginal
{
    public class SelectOriginalCommandHandler : IRequestHandler<SelectOriginalCommand, bool>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<SelectOriginalCommandHandler> _logger;

        public SelectOriginalCommandHandler(IOrderRepository postRepository, IHttpContextAccessor httpContextAccessor,
            ILogger<SelectOriginalCommandHandler> logger)
        {
            _orderRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(SelectOriginalCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetOrderWithAttachmentsAsync(request.OrderId);
            order.SelectOriginalFiles(request.Attachments);
            return await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
