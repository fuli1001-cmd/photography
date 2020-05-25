using Arise.DDD.Domain.Exceptions;
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

namespace Photography.Services.Post.API.Application.Commands.AppointmentDeal.AppointTask
{
    public class AppointTaskCommandHandler : IRequestHandler<AppointTaskCommand, AppointmentViewModel>
    {
        private readonly IPostRepository _postRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly ILogger<AppointTaskCommandHandler> _logger;
        private readonly IServiceProvider _serviceProvider;

        private IMessageSession _messageSession;

        public AppointTaskCommandHandler(IPostRepository postRepository, IHttpContextAccessor httpContextAccessor,
            IServiceProvider serviceProvider, IMapper mapper, ILogger<AppointTaskCommandHandler> logger)
        {
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<AppointmentViewModel> Handle(AppointTaskCommand request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var attachments = request.Attachments.Select(a => new PostAttachment(a.Name, a.Text, a.AttachmentType)).ToList();
            var appointment = await _postRepository.GetByIdAsync(request.AppointmentId);
            
            PayerType payerType = PayerType.Free;
            if (appointment.PayerType == PayerType.Me)
                payerType = PayerType.You;
            else if (appointment.PayerType == PayerType.You)
                payerType = PayerType.Me;

            var deal = Domain.AggregatesModel.PostAggregate.Post.CreateAppointmentDeal(
                request.Text, appointment.AppointedTime, appointment.Price, payerType, 
                appointment.Latitude, appointment.Longitude, appointment.LocationName, appointment.Address, 
                appointment.CityCode, attachments, Guid.Parse(userId), appointment.UserId, appointment.Id);
            _postRepository.Add(deal);

            if (await _postRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
                await SendAppointmentDealConfirmedEventAsync(deal);

            _postRepository.LoadUser(deal);
            return _mapper.Map<AppointmentViewModel>(deal);
        }

        private async Task SendAppointmentDealConfirmedEventAsync(Domain.AggregatesModel.PostAggregate.Post deal)
        {
            var @event = new AppointmentDealCreatedEvent
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
            _logger.LogInformation("----- Published AppointmentDealCreatedEvent: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);
        }
    }
}
