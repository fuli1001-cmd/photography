using MediatR;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.Domain.AggregatesModel.UserPostRelationAggregate;
using Photography.Services.Post.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.DomainEventHandlers.DeletedPost
{
    public class DeleteUserPostRelationWhenDeletedPostDomainEventHandler : INotificationHandler<DeletedPostDomainEvent>
    {
        private readonly IUserPostRelationRepository _userPostRelationRepository;
        private readonly ILogger<DeleteUserPostRelationWhenDeletedPostDomainEventHandler> _logger;

        public DeleteUserPostRelationWhenDeletedPostDomainEventHandler(
            IUserPostRelationRepository userPostRelationRepository,
            ILogger<DeleteUserPostRelationWhenDeletedPostDomainEventHandler> logger)
        {
            _userPostRelationRepository = userPostRelationRepository ?? throw new ArgumentNullException(nameof(userPostRelationRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(DeletedPostDomainEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("----- Handling DeletedPostDomainEvent: at {AppName} - ({@DomainEvent})", Program.AppName, notification);

            var relations = await _userPostRelationRepository.GetRelationsByPostIdAsync(notification.PostId);

            relations.ForEach(r => _userPostRelationRepository.Remove(r));
        }
    }
}
