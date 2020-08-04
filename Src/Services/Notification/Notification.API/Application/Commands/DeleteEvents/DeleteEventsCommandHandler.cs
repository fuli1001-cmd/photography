using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Photography.Services.Notification.Domain.AggregatesModel.EventAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Notification.API.Application.Commands.DeleteEvents
{
    public class DeleteEventsCommandHandler : IRequestHandler<DeleteEventsCommand, bool>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<DeleteEventsCommandHandler> _logger;

        public DeleteEventsCommandHandler(IEventRepository eventRepository, IHttpContextAccessor httpContextAccessor, ILogger<DeleteEventsCommandHandler> logger)
        {
            _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(DeleteEventsCommand request, CancellationToken cancellationToken)
        {
            List<Event> events = null;
            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (request.EventCategory != null)
                events = await _eventRepository.GetUserCategoryEventsAsync(myId, request.EventCategory.Value);
            else if (request.EventIds != null && request.EventIds.Count > 0)
                events = await _eventRepository.GetEventsAsync(request.EventIds);

            if (events != null)
            {
                events.ForEach(e => _eventRepository.Remove(e));
                
                if (await _eventRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
                    return true;

                throw new ApplicationException("操作失败");
            }

            return true;
        }
    }
}
