using ApplicationMessages.Events.Order;
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

namespace Photography.Services.Order.API.Application.Commands.ConfirmShot
{
    public class ConfirmShotCommandHandler : IRequestHandler<ConfirmShotCommand, OrderViewModel>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<ConfirmShotCommandHandler> _logger;
        private readonly IOrderQueries _orderQueries;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private IMessageSession _messageSession;

        public ConfirmShotCommandHandler(IOrderQueries orderQueries, 
            IOrderRepository orderRepository,
            IServiceProvider serviceProvider,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ConfirmShotCommandHandler> logger)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _orderQueries = orderQueries ?? throw new ArgumentNullException(nameof(orderQueries));
        }

        public async Task<OrderViewModel> Handle(ConfirmShotCommand request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var order = await _orderRepository.GetByIdAsync(request.OrderId);

            if (order.User1Id != userId && order.User2Id != userId)
                throw new ClientException("操作失败", new List<string> { $"Current user {userId} is not the owner of order {order.Id}" });

            order.ConfirmShot();

            if (await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
                await SendConfirmShotEventAsync(userId, order.User1Id == userId ? order.User2Id : order.User1Id, order.Id);

            return await _orderQueries.GetOrderAsync(order.Id);
        }

        private async Task SendConfirmShotEventAsync(Guid userId, Guid anotherUserId, Guid orderId)
        {
            var @event = new ConfirmShotEvent { ConfirmUserId = userId, AnotherUserId = anotherUserId, OrderId = orderId };
            _messageSession = (IMessageSession)_serviceProvider.GetService(typeof(IMessageSession));
            await _messageSession.Publish(@event);
            _logger.LogInformation("----- Published ConfirmShotEvent: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);
        }
    }
}
