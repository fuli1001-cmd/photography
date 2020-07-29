using Arise.DDD.Domain.Exceptions;
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

namespace Photography.Services.Notification.API.Application.Commands.ReadEvent
{
    public class ReadEventCommandHandler : IRequestHandler<ReadEventCommand, bool>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ReadEventCommandHandler> _logger;

        public ReadEventCommandHandler(IEventRepository eventRepository, IHttpContextAccessor httpContextAccessor, ILogger<ReadEventCommandHandler> logger)
        {
            _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ReadEventCommand request, CancellationToken cancellationToken)
        {
            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var events = await _eventRepository.GetUserUnReadCategoryEventsAsync(myId, request.EventCategory);

            events.ForEach(e => e.MarkAsReaded());

            if (await _eventRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
                return true;

            throw new ApplicationException("操作失败");
        }
    }
}
