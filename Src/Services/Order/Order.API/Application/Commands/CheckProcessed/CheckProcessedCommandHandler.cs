using ApplicationMessages.Events.Order;
using Arise.DDD.Domain.Exceptions;
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

namespace Photography.Services.Order.API.Application.Commands.CheckProcessed
{
    public class CheckProcessedCommandHandler : IRequestHandler<CheckProcessedCommand, OrderViewModel>
    {
        private readonly IOrderQueries _orderQueries;
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<CheckProcessedCommandHandler> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IServiceProvider _serviceProvider;

        private IMessageSession _messageSession;

        public CheckProcessedCommandHandler(
            IOrderQueries orderQueries, 
            IOrderRepository postRepository,
            IHttpContextAccessor httpContextAccessor,
            IServiceProvider serviceProvider, 
            ILogger<CheckProcessedCommandHandler> logger)
        {
            _orderQueries = orderQueries ?? throw new ArgumentNullException(nameof(orderQueries));
            _orderRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OrderViewModel> Handle(CheckProcessedCommand request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var order = await _orderRepository.GetByIdAsync(request.OrderId);

            if (order.User1Id != userId && order.User2Id != userId)
                throw new ClientException("操作失败", new List<string> { $"Current user {userId} is not the owner of order {order.Id}" });

            order.AcceptProcessedFiles();

            if (await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
            {
                await SendOrderFinishedEventAsync(userId, order.User1Id == userId ? order.User2Id : order.User1Id, order.Id);
                return await _orderQueries.GetOrderAsync(order.Id);
            }

            throw new ApplicationException("操作失败");
        }

        private async Task SendOrderFinishedEventAsync(Guid acceptUserId, Guid anotherUserId, Guid orderId)
        {
            var @event = new OrderFinishedEvent { AcceptUserId = acceptUserId, AnotherUserId = anotherUserId, OrderId = orderId };
            _messageSession = (IMessageSession)_serviceProvider.GetService(typeof(IMessageSession));
            await _messageSession.Publish(@event);
            _logger.LogInformation("----- Published OrderFinishedEvent: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);
        }
    }
}
