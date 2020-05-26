using MediatR;
using Microsoft.Extensions.Logging;
using Photography.Services.Order.Domain.AggregatesModel.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Order.API.Application.Commands.UploadProcessed
{
    public class UploadProcessedCommandHandler : IRequestHandler<UploadProcessedCommand, bool>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<UploadProcessedCommandHandler> _logger;

        public UploadProcessedCommandHandler(IOrderRepository postRepository, ILogger<UploadProcessedCommandHandler> logger)
        {
            _orderRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(UploadProcessedCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetOrderWithAttachmentsAsync(request.OrderId);
            var attachments = request.Attachments.Select(name => new Attachment(name));
            order.UploadProcessedFiles(attachments);
            return await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
