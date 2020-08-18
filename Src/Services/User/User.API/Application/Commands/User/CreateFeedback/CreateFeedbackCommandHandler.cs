using ApplicationMessages.Events.User;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.User.Domain.AggregatesModel.FeedbackAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.User.CreateFeedback
{
    public class CreateFeedbackCommandHandler : IRequestHandler<CreateFeedbackCommand, bool>
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CreateFeedbackCommandHandler> _logger;
        private readonly IServiceProvider _serviceProvider;

        private IMessageSession _messageSession;

        public CreateFeedbackCommandHandler(
            IFeedbackRepository feedbackRepository,
            IHttpContextAccessor httpContextAccessor,
            IServiceProvider serviceProvider,
            ILogger<CreateFeedbackCommandHandler> logger)
        {
            _feedbackRepository = feedbackRepository ?? throw new ArgumentNullException(nameof(feedbackRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(CreateFeedbackCommand request, CancellationToken cancellationToken)
        {
            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var feedback = new Feedback(request.Text, request.Image1, request.Image2, request.Image3, myId);

            _feedbackRepository.Add(feedback);

            if (await _feedbackRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
            {
                await SendUserFeedbackCreatedEventAsync(myId);
                return true;
            }

            throw new ApplicationException("操作失败");
        }

        private async Task SendUserFeedbackCreatedEventAsync(Guid userId)
        {
            var @event = new UserFeedbackCreatedEvent { UserId = userId };
            _messageSession = (IMessageSession)_serviceProvider.GetService(typeof(IMessageSession));
            await _messageSession.Publish(@event);
            _logger.LogInformation("----- Published UserFeedbackCreatedEvent: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);
        }
    }
}
