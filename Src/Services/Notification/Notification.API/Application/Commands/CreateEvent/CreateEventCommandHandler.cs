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
            _logger.LogInformation("****************** CreateEventCommandHandler 1");

            // 不记录自己给自己产生的事件
            if (request.FromUserId != request.ToUserId)
            {
                _logger.LogInformation("****************** CreateEventCommandHandler 2");
                var @event = new Event(request.FromUserId, request.ToUserId, request.EventType, request.PostId, request.CommentId, request.CommentText);
                _logger.LogInformation("****************** CreateEventCommandHandler 3");
                _eventRepository.Add(@event);
                _logger.LogInformation("****************** CreateEventCommandHandler 4");
                var result = await _eventRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
                _logger.LogInformation("****************** CreateEventCommandHandler 5 result: {result}", result);
            }
            _logger.LogInformation("****************** CreateEventCommandHandler 6");
            return true;
        }
    }
}
