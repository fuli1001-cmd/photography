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

namespace Photography.Services.Order.API.Application.Commands.UploadProcessed
{
    public class UploadProcessedCommandHandler : IRequestHandler<UploadProcessedCommand, OrderViewModel>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<UploadProcessedCommandHandler> _logger;
        private readonly IOrderQueries _orderQueries;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IServiceProvider _serviceProvider;
        private IMessageSession _messageSession;

        public UploadProcessedCommandHandler(
            IOrderRepository orderRepository, 
            IOrderQueries orderQueries,
            IServiceProvider serviceProvider,
            IHttpContextAccessor httpContextAccessor,
            ILogger<UploadProcessedCommandHandler> logger)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _orderQueries = orderQueries ?? throw new ArgumentNullException(nameof(orderQueries));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<OrderViewModel> Handle(UploadProcessedCommand request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var order = await _orderRepository.GetOrderWithAttachmentsAsync(request.OrderId);

            if (order.User1Id != userId && order.User2Id != userId)
                throw new ClientException("操作失败", new List<string> { $"Current user {userId} is not the owner of order {order.Id}" });

            order.UploadProcessedFiles(request.Attachments);
            
            if (await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
                await SendProcessedPhotoUploadedEventAsync(userId, order.User1Id == userId ? order.User2Id : order.User1Id, order.Id);

            return await _orderQueries.GetOrderAsync(order.Id);
        }

        private async Task SendProcessedPhotoUploadedEventAsync(Guid uploadPhotoUserId, Guid anotherUserId, Guid orderId)
        {
            var @event = new ProcessedPhotoUploadedEvent { UploadPhotoUserId = uploadPhotoUserId, AnotherUserId = anotherUserId, OrderId = orderId };
            _messageSession = (IMessageSession)_serviceProvider.GetService(typeof(IMessageSession));
            await _messageSession.Publish(@event);
            _logger.LogInformation("----- Published ProcessedPhotoUploadedEvent: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);
        }
    }
}
