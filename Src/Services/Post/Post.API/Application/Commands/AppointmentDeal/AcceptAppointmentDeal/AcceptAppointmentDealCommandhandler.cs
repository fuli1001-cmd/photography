using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Messages.Events;
using Photography.Services.Post.API.Query.ViewModels;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.AppointmentDeal.AcceptAppointmentDeal
{
    public class AcceptAppointmentDealCommandhandler : IRequestHandler<AcceptAppointmentDealCommand, AppointmentViewModel>
    {
        private readonly IPostRepository _postRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly ILogger<AcceptAppointmentDealCommandhandler> _logger;
        private readonly IServiceProvider _serviceProvider;

        private IMessageSession _messageSession;

        public AcceptAppointmentDealCommandhandler(IPostRepository postRepository, IHttpContextAccessor httpContextAccessor,
            IServiceProvider serviceProvider, IMapper mapper, ILogger<AcceptAppointmentDealCommandhandler> logger)
        {
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<AppointmentViewModel> Handle(AcceptAppointmentDealCommand request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var deal = await _postRepository.GetByIdAsync(request.AppointmentId);
            deal.AcceptAppointmentDeal(userId);

            if (await _postRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
                await SendAppointmentDealRejectedEventAsync(deal);

            _postRepository.LoadUser(deal);
            return _mapper.Map<AppointmentViewModel>(deal);
        }

        private async Task SendAppointmentDealRejectedEventAsync(Domain.AggregatesModel.PostAggregate.Post deal)
        {
            var @event = new AppointmentDealAcceptedEvent { DealId = deal.Id };
            _messageSession = (IMessageSession)_serviceProvider.GetService(typeof(IMessageSession));
            await _messageSession.Publish(@event);
            _logger.LogInformation("----- Published AppointmentDealAcceptedEvent: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);
        }
    }
}
