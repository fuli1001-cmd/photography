using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Messages.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Order.API.Application.IntegrationEvents
{
    public class AppointmentDealCanceledEventHandler : IHandleMessages<AppointmentDealCanceledEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AppointmentDealCanceledEventHandler> _logger;

        public AppointmentDealCanceledEventHandler(IMediator mediator, ILogger<AppointmentDealCanceledEventHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task Handle(AppointmentDealCanceledEvent message, IMessageHandlerContext context)
        {
            throw new NotImplementedException();    
        }
    }
}
