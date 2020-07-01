using MediatR;
using Microsoft.Extensions.Logging;
using Photography.Services.Notification.Domain.AggregatesModel.EventAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Notification.API.Application.Commands.ProcessEvent
{
    public class ProcessEventCommandHandler : IRequestHandler<ProcessEventCommand, bool>
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILogger<ProcessEventCommandHandler> _logger;

        public ProcessEventCommandHandler(IEventRepository eventRepository, ILogger<ProcessEventCommandHandler> logger)
        {
            _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessEventCommand request, CancellationToken cancellationToken)
        {
            var events = await _eventRepository.GetUnProcessedEventsAsync(request.FromUserId, request.ToUserId, request.EventType);
            events.ForEach(e => e.MarkAsProcessed());
            return await _eventRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
