using ApplicationMessages.Events;
using Arise.DDD.Domain.Exceptions;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.Post.API.Query.Interfaces;
using Photography.Services.Post.API.Query.ViewModels;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.Appointment.PublishAppointment
{
    public class PublishAppointmentCommandHandler : IRequestHandler<PublishAppointmentCommand, AppointmentViewModel>
    {
        private readonly IPostRepository _postRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAppointmentQueries _appointmentQueries;
        private readonly ILogger<PublishAppointmentCommandHandler> _logger;
        private readonly IServiceProvider _serviceProvider;

        private IMessageSession _messageSession;

        public PublishAppointmentCommandHandler(IPostRepository postRepository,
            IServiceProvider serviceProvider, 
            IHttpContextAccessor httpContextAccessor,
            IAppointmentQueries appointmentQueries, ILogger<PublishAppointmentCommandHandler> logger)
        {
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _appointmentQueries = appointmentQueries ?? throw new ArgumentNullException(nameof(appointmentQueries));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<AppointmentViewModel> Handle(PublishAppointmentCommand request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var attachments = request.Attachments.Select(a => new PostAttachment(a.Name, a.Text, a.AttachmentType)).ToList();
            var post = Domain.AggregatesModel.PostAggregate.Post.CreateAppointment(request.Text, request.AppointedTime, request.Price, request.PayerType,
                request.Latitude, request.Longitude, request.LocationName, request.Address, request.CityCode, attachments, userId);
            _postRepository.Add(post);

            if (await _postRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
            {
                await SendAppointmentPublishedEventAsync(userId);
                return await _appointmentQueries.GetAppointmentAsync(post.Id);
            }

            throw new ApplicationException("操作失败。");
        }

        private async Task SendAppointmentPublishedEventAsync(Guid userId)
        {
            var @event = new AppointmentPublishedEvent { UserId = userId };
            _messageSession = (IMessageSession)_serviceProvider.GetService(typeof(IMessageSession));
            await _messageSession.Publish(@event);

            _logger.LogInformation("----- Published AppointmentPublishedEvent: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);
        }
    }
}
