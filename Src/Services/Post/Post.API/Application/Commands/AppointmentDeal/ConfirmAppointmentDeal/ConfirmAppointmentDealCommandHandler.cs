using Arise.DDD.Domain.Exceptions;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Messages.Events;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.AppointmentDeal.ConfirmAppointmentDeal
{
    public class ConfirmAppointmentDealCommandHandler : IRequestHandler<ConfirmAppointmentDealCommand, bool>
    {
        private readonly IPostRepository _postRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ConfirmAppointmentDealCommandHandler> _logger;

        private IMessageSession _messageSession;

        public ConfirmAppointmentDealCommandHandler(IPostRepository postRepository, IHttpContextAccessor httpContextAccessor,
            IServiceProvider serviceProvider, ILogger<ConfirmAppointmentDealCommandHandler> logger)
        {
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ConfirmAppointmentDealCommand request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var deal = await _postRepository.GetByIdAsync(request.AppointmentId);
            deal.CanfirmAppointmentDeal(userId);
            if (await _postRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
            {
                await SendAppointmentDealConfirmedEventAsync(deal);
                return true;
            }
            else
                return false;
        }

        private async Task SendAppointmentDealConfirmedEventAsync(Domain.AggregatesModel.PostAggregate.Post deal)
        {
            var @event = new AppointmentDealConfirmedEvent
            {
                User1Id = deal.UserId,
                User2Id = deal.AppointmentedUserId ?? throw new DomainException("约拍对象不能为空"),
                DealId = deal.Id,
                Price = deal.Price ?? 0,
                AppointedTime = deal.AppointedTime ?? throw new DomainException("约拍时间不能为空"),
                PayerId = deal.PayerType == PayerType.Free ? null : (deal.PayerType == PayerType.Me ? deal.UserId : deal.AppointmentedUserId),
                Text = deal.Text,
                Latitude = deal.Latitude ?? throw new DomainException("约拍纬度不能为空"),
                Longitude = deal.Longitude ?? throw new DomainException("约拍经度不能为空"),
                LocationName = deal.LocationName,
                Address = deal.Address
            };
            _messageSession = (IMessageSession)_serviceProvider.GetService(typeof(IMessageSession));
            await _messageSession.Publish(@event);
            _logger.LogInformation("----- Published AppointmentDealConfirmedEvent: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);
        }
    }
}
