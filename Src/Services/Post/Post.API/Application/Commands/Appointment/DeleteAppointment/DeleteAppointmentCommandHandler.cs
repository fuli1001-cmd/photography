using ApplicationMessages.Events;
using Arise.DDD.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.Appointment.DeleteAppointment
{
    public class DeleteAppointmentCommandHandler : IRequestHandler<DeleteAppointmentCommand, bool>
    {
        private readonly IPostRepository _postRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<DeleteAppointmentCommandHandler> _logger;
        private readonly IServiceProvider _serviceProvider;

        private IMessageSession _messageSession;

        public DeleteAppointmentCommandHandler(IPostRepository postRepository,
            IHttpContextAccessor httpContextAccessor,
            IServiceProvider serviceProvider,
            ILogger<DeleteAppointmentCommandHandler> logger)
        {
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(DeleteAppointmentCommand request, CancellationToken cancellationToken)
        {
            var post = await _postRepository.GetAppointmentById(request.AppointmentId);

            if (post == null)
                throw new DomainException("删除失败。");

            var userId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // 当前用户不是发布该帖的用户，不能删除
            if (post.UserId != userId)
                throw new DomainException("删除失败。");

            post.Delete();
            _postRepository.Update(post);
            _postRepository.Remove(post);

            if (await _postRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
            {
                await SendAppointmentDeletedEventAsync(userId);
                return true;
            }
            else
                throw new DomainException("删除失败。");
        }

        private async Task SendAppointmentDeletedEventAsync(Guid userId)
        {
            var @event = new AppointmentDeletedEvent { UserId = userId };
            _messageSession = (IMessageSession)_serviceProvider.GetService(typeof(IMessageSession));
            await _messageSession.Publish(@event);
            _logger.LogInformation("----- Published AppointmentDeletedEvent: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);
        }
    }
}
