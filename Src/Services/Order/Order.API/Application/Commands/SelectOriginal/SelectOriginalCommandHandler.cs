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

namespace Photography.Services.Order.API.Application.Commands.SelectOriginal
{
    public class SelectOriginalCommandHandler : IRequestHandler<SelectOriginalCommand, OrderViewModel>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderQueries _orderQueries;
        private readonly ILogger<SelectOriginalCommandHandler> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IServiceProvider _serviceProvider;
        private IMessageSession _messageSession;

        public SelectOriginalCommandHandler(IOrderQueries orderQueries, 
            IOrderRepository orderRepository,
            IServiceProvider serviceProvider,
            IHttpContextAccessor httpContextAccessor,
            ILogger<SelectOriginalCommandHandler> logger)
        {
            _orderQueries = orderQueries ?? throw new ArgumentNullException(nameof(orderQueries));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OrderViewModel> Handle(SelectOriginalCommand request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var order = await _orderRepository.GetOrderWithAttachmentsAsync(request.OrderId);

            if (order.User1Id != userId && order.User2Id != userId)
                throw new ClientException("操作失败", new List<string> { $"Current user {userId} is not the owner of order {order.Id}" });

            order.SelectOriginalFiles(request.Attachments);

            if (await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
                await SendOriginalPhotoSelectedEventAsync(userId, order.User1Id == userId ? order.User2Id : order.User1Id, order.Id);

            return await _orderQueries.GetOrderAsync(order.Id);
        }

        private async Task SendOriginalPhotoSelectedEventAsync(Guid selectPhotoUserId, Guid anotherUserId, Guid orderId)
        {
            var @event = new OriginalPhotoSelectedEvent { SelectPhotoUserId = selectPhotoUserId, AnotherUserId = anotherUserId, OrderId = orderId };
            _messageSession = (IMessageSession)_serviceProvider.GetService(typeof(IMessageSession));
            await _messageSession.Publish(@event);
            _logger.LogInformation("----- Published OriginalPhotoSelectedEvent: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);
        }
    }
}
