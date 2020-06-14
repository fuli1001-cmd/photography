using ApplicationMessages.Events;
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
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Order.API.Application.Commands.CheckProcessed
{
    public class CheckProcessedCommandHandler : IRequestHandler<CheckProcessedCommand, OrderViewModel>
    {
        private readonly IOrderQueries _orderQueries;
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<CheckProcessedCommandHandler> _logger;
        private readonly IServiceProvider _serviceProvider;

        private IMessageSession _messageSession;

        public CheckProcessedCommandHandler(
            IOrderQueries orderQueries, 
            IOrderRepository postRepository,
            IServiceProvider serviceProvider, 
            ILogger<CheckProcessedCommandHandler> logger)
        {
            _orderQueries = orderQueries ?? throw new ArgumentNullException(nameof(orderQueries));
            _orderRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OrderViewModel> Handle(CheckProcessedCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(request.OrderId);
            order.AcceptProcessedFiles();
            if (await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
            {
                await SendOrderFinishedEventAsync(order.User1Id, order.User2Id);
                return await _orderQueries.GetOrderAsync(order.Id);
            }
            throw new ApplicationException("操作失败。");
        }

        private async Task SendOrderFinishedEventAsync(Guid user1Id, Guid user2Id)
        {
            var @event = new OrderFinishedEvent { User1Id = user1Id, User2Id = user2Id };
            _messageSession = (IMessageSession)_serviceProvider.GetService(typeof(IMessageSession));
            await _messageSession.Publish(@event);
            _logger.LogInformation("----- Published OrderFinishedEvent: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);
        }
    }
}
