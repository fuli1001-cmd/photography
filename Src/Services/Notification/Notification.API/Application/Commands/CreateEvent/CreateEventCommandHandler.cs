using MediatR;
using Microsoft.Extensions.Logging;
using Photography.Services.Notification.Domain.AggregatesModel.EventAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Notification.API.Application.Commands.CreateEvent
{
    public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, bool>
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILogger<CreateEventCommandHandler> _logger;

        public CreateEventCommandHandler(IEventRepository eventRepository, ILogger<CreateEventCommandHandler> logger)
        {
            _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(CreateEventCommand request, CancellationToken cancellationToken)
        {
            // 不记录自己给自己产生的事件
            if (request.FromUserId != request.ToUserId)
            {
                var @event = new Event(request.FromUserId, request.ToUserId, request.EventType, request.PostId, 
                    request.CommentId, request.CommentText, request.CircleId, request.CircleName, request.OrderId);
                _eventRepository.Add(@event);
                var result = await _eventRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
            }
            return true;
        }
    }
}
