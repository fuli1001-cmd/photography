using Arise.DDD.Domain.Exceptions;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Messages.Events;
using Photography.Services.Order.API.Query.ViewModels;
using Photography.Services.Order.Domain.AggregatesModel.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Order.API.Application.Commands.RejectOrder
{
    public class RejectOrderCommandHandler : IRequestHandler<RejectOrderCommand, OrderViewModel>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<RejectOrderCommandHandler> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMapper _mapper;

        private IMessageSession _messageSession;

        public RejectOrderCommandHandler(IOrderRepository postRepository, IServiceProvider serviceProvider,
            IMapper mapper, ILogger<RejectOrderCommandHandler> logger)
        {
            _orderRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<OrderViewModel> Handle(RejectOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetbyDealIdAsync(request.DealId);
            if (order == null)
                throw new DomainException("没有与当前约拍交易对应的订单。");

            order.Reject();

            if (await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
                await SendOrderRejectedEventAsync(request.DealId);

            return _mapper.Map<OrderViewModel>(order);
        }

        private async Task SendOrderRejectedEventAsync(Guid dealId)
        {
            var @event = new OrderRejectedEvent { DealId = dealId };
            _messageSession = (IMessageSession)_serviceProvider.GetService(typeof(IMessageSession));
            await _messageSession.Publish(@event);
            _logger.LogInformation("----- Published OrderRejectedEvent: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);
        }
    }
}
