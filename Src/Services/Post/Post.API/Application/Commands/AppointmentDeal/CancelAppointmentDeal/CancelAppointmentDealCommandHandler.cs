using Arise.DDD.Domain.Exceptions;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Photography.Messages.Events;
using NServiceBus;
using Photography.Services.Post.API.Query.ViewModels;

namespace Photography.Services.Post.API.Application.Commands.AppointmentDeal.CancelAppointmentDeal
{
    public class CancelAppointmentDealCommandHandler : IRequestHandler<CancelAppointmentDealCommand, AppointmentViewModel>
    {
        private readonly IPostRepository _postRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly ILogger<CancelAppointmentDealCommandHandler> _logger;
        private readonly IServiceProvider _serviceProvider;

        private IMessageSession _messageSession;

        public CancelAppointmentDealCommandHandler(IPostRepository postRepository, IHttpContextAccessor httpContextAccessor,
            IServiceProvider serviceProvider, IMapper mapper, ILogger<CancelAppointmentDealCommandHandler> logger)
        {
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<AppointmentViewModel> Handle(CancelAppointmentDealCommand request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var deal = await _postRepository.GetByIdAsync(request.AppointmentId);
            deal.CancelAppointmentDeal(userId);

            if (await _postRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
                await SendAppointmentDealCanceledEventAsync(deal);

            _postRepository.LoadUser(deal);
            return _mapper.Map<AppointmentViewModel>(deal);
        }

        private async Task SendAppointmentDealCanceledEventAsync(Domain.AggregatesModel.PostAggregate.Post deal)
        {
            var @event = new AppointmentDealCanceledEvent { DealId = deal.Id };
            _messageSession = (IMessageSession)_serviceProvider.GetService(typeof(IMessageSession));
            await _messageSession.Publish(@event);
            _logger.LogInformation("----- Published AppointmentDealCanceledEvent: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);
        }
    }
}
