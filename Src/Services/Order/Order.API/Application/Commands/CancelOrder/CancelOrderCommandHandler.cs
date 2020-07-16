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

namespace Photography.Services.Order.API.Application.Commands.CancelOrder
{
    public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, OrderViewModel>
    {
        private readonly IOrderQueries _orderQueries;
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<CancelOrderCommandHandler> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private IMessageSession _messageSession;

        public CancelOrderCommandHandler(IOrderQueries orderQueries, IOrderRepository orderRepository, IServiceProvider serviceProvider,
            IHttpContextAccessor httpContextAccessor, ILogger<CancelOrderCommandHandler> logger)
        {
            _orderQueries = orderQueries ?? throw new ArgumentNullException(nameof(orderQueries));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<OrderViewModel> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetOrderbyDealIdAsync(request.DealId);
            if (order == null)
                throw new ClientException("操作失败", new List<string> { $"No order for deal id {request.DealId}." });

            var processingUserId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (processingUserId != order.User1Id && processingUserId != order.User2Id)
                throw new ClientException("操作失败", new List<string> { $"Order does not belong to user {order.User1Id} and user {order.User2Id}." });

            order.Cancel(request.Description);

            if (await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
            {
                var anotherUserId = processingUserId == order.User1Id ? order.User2Id : order.User1Id;
                await SendOrderCanceledEventAsync(processingUserId, anotherUserId, request.DealId, order.Id, request.Description);
            }

            return await _orderQueries.GetOrderAsync(order.Id);
        }

        private async Task SendOrderCanceledEventAsync(Guid processingUserId, Guid anotherUserId, Guid dealId, Guid orderId, string description)
        {
            var @event = new OrderCanceledEvent 
            { 
                ProcessingUserId = processingUserId, 
                AnotherUserId = anotherUserId, 
                DealId = dealId, 
                OrderId = orderId,
                Description = description 
            };
            _messageSession = (IMessageSession)_serviceProvider.GetService(typeof(IMessageSession));
            await _messageSession.Publish(@event);
            _logger.LogInformation("----- Published OrderCanceledEvent: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);
        }
    }
}
