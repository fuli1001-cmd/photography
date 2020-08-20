using ApplicationMessages.Events;
using Arise.DDD.Domain.Exceptions;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NServiceBus;
using Photography.Services.Post.API.Query.Interfaces;
using Photography.Services.Post.API.Query.ViewModels;
using Photography.Services.Post.API.Settings;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using Photography.Services.Post.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.AppointmentDeal.AppointUser
{
    public class AppointUserCommandHandler : IRequestHandler<AppointUserCommand, AppointmentViewModel>
    {
        private readonly IPostRepository _postRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAppointmentDealQueries _appointmentDealQueries;
        private readonly AppointmentSettings _appointmentSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly ILogger<AppointUserCommandHandler> _logger;
        private readonly IServiceProvider _serviceProvider;

        private IMessageSession _messageSession;

        public AppointUserCommandHandler(
            IPostRepository postRepository,
            IUserRepository userRepository,
            IAppointmentDealQueries appointmentDealQueries,
            IOptionsSnapshot<AppointmentSettings> appointmentOptions, 
            IHttpContextAccessor httpContextAccessor,
            IServiceProvider serviceProvider, 
            IMapper mapper, 
            ILogger<AppointUserCommandHandler> logger)
        {
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _appointmentDealQueries = appointmentDealQueries ?? throw new ArgumentNullException(nameof(appointmentDealQueries));
            _appointmentSettings = appointmentOptions?.Value ?? throw new ArgumentNullException(nameof(appointmentOptions));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<AppointmentViewModel> Handle(AppointUserCommand request, CancellationToken cancellationToken)
        {
            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if ((await _postRepository.GetTodayUserSentAppointmentDealCountAsync(myId)) >= _appointmentSettings.MaxSendDealCount)
                throw new ClientException("已达今日最大约拍发起数量");

            if ((await _postRepository.GetTodayUserReceivedAppointmentDealCountAsync(request.AppointmentedUserId)) >= _appointmentSettings.MaxReceiveDealCount)
                throw new ClientException("对方已达今日最大被约数量");

            // 创建约拍交易
            var attachments = request.Attachments.Select(a => new PostAttachment(a.Name, a.Text, a.AttachmentType)).ToList();

            var deal = Domain.AggregatesModel.PostAggregate.Post.CreateAppointmentDeal(
                request.Text, request.AppointedTime, request.Price, request.PayerType, request.AppointmentedUserType, 
                request.Latitude, request.Longitude, request.LocationName, request.Address, request.CityCode, attachments, 
                myId, request.AppointmentedUserId, null);
            _postRepository.Add(deal);

            // 增加约拍发起人和被约拍人的约拍值
            var users = await _userRepository.GetUsersAsync(new List<Guid> { myId, request.AppointmentedUserId });
            users.FirstOrDefault(u => u.Id == myId)?.AddAppointmentScore(_appointmentSettings.SendDealScore);
            users.FirstOrDefault(u => u.Id == request.AppointmentedUserId)?.AddAppointmentScore(_appointmentSettings.ReceiveDealScore);

            // 发布事件
            if (await _postRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
            {
                //await SendAppointmentDealConfirmedEventAsync(deal);
                var eventTasks = new List<Task>
                {
                    SendAppointmentDealCreatedEventAsync(deal),
                    SendAppointmentScoreChangedEventAsync(myId, _appointmentSettings.SendDealScore),
                    SendAppointmentScoreChangedEventAsync(request.AppointmentedUserId, _appointmentSettings.ReceiveDealScore)
                };
                await Task.WhenAll(eventTasks);
            }

            return await _appointmentDealQueries.GetSentAppointmentDealAsync(deal.Id);

            //_postRepository.LoadUser(deal);
            //return _mapper.Map<AppointmentViewModel>(deal);
        }

        private async Task SendAppointmentDealCreatedEventAsync(Domain.AggregatesModel.PostAggregate.Post deal)
        {
            var @event = new AppointmentDealCreatedEvent
            {
                User1Id = deal.UserId,
                User2Id = deal.AppointmentedUserId.Value,
                // user1是约拍发出方，他的角色应该和约拍时希望约的角色相反
                User1Type = (int)(deal.AppointmentedUserType == AppointmentedUserType.Model ? AppointmentedUserType.Photographer : AppointmentedUserType.Model),
                // user2是约拍接收方，他的角色就是约拍时希望约的角色
                User2Type = (int)deal.AppointmentedUserType,
                DealId = deal.Id,
                AppointmentedUserType = (int)deal.AppointmentedUserType,
                PayerType = (int)deal.PayerType,
                Price = deal.Price ?? 0,
                AppointedTime = deal.AppointedTime.Value,
                PayerId = deal.PayerType == PayerType.Free ? null : (deal.PayerType == PayerType.Me ? deal.UserId : deal.AppointmentedUserId),
                Text = deal.Text,
                Latitude = deal.Latitude.Value,
                Longitude = deal.Longitude.Value,
                LocationName = deal.LocationName,
                Address = deal.Address
            };
            _messageSession = (IMessageSession)_serviceProvider.GetService(typeof(IMessageSession));
            await _messageSession.Publish(@event);
            _logger.LogInformation("----- Published AppointmentDealCreatedEvent: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);
        }

        private async Task SendAppointmentScoreChangedEventAsync(Guid userId, int score)
        {
            var @event = new AppointmentScoreChangedEvent { UserId = userId, ChangedScore = score };
            _messageSession = (IMessageSession)_serviceProvider.GetService(typeof(IMessageSession));
            await _messageSession.Publish(@event);

            _logger.LogInformation("----- Published AppointmentScoreChangedEvent: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);
        }
    }
}
