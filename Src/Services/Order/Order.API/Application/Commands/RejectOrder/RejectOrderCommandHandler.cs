using ApplicationMessages.Events;
using Arise.DDD.Domain.Exceptions;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.Order.API.Query.Interfaces;
using Photography.Services.Order.API.Query.ViewModels;
using Photography.Services.Order.Domain.AggregatesModel.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Order.API.Application.Commands.RejectOrder
{
    public class RejectOrderCommandHandler : IRequestHandler<RejectOrderCommand, OrderViewModel>
    {
        private readonly IOrderQueries _orderQueries;
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<RejectOrderCommandHandler> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private IMessageSession _messageSession;

        public RejectOrderCommandHandler(IOrderQueries orderQueries, IOrderRepository orderRepository, IServiceProvider serviceProvider,
            IHttpContextAccessor httpContextAccessor, ILogger<RejectOrderCommandHandler> logger)
        {
            _orderQueries = orderQueries ?? throw new ArgumentNullException(nameof(orderQueries));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<OrderViewModel> Handle(RejectOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetOrderbyDealIdAsync(request.DealId);
            if (order == null)
                throw new DomainException("没有与当前约拍交易对应的订单。");

            order.Reject();

            if (await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
            {
                var userId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                await SendOrderRejectedEventAsync(userId, request.DealId);
            }

            return await _orderQueries.GetOrderAsync(order.Id);
        }

        private async Task SendOrderRejectedEventAsync(Guid userId, Guid dealId)
        {
            var @event = new OrderRejectedEvent { UserId = userId, DealId = dealId };
            _messageSession = (IMessageSession)_serviceProvider.GetService(typeof(IMessageSession));
            await _messageSession.Publish(@event);
            _logger.LogInformation("----- Published OrderRejectedEvent: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);
        }
    }
}
