using MediatR;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.Domain.AggregatesModel.UserCommentRelationAggregate;
using Photography.Services.Post.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.DomainEventHandlers.DeletedPost
{
    public class DeleteUserCommentRelationWhenDeletedPostDomainEventHandler : INotificationHandler<DeletedPostDomainEvent>
    {
        private readonly IUserCommentRelationRepository _userCommentRelationRepository;
        private readonly ILogger<DeleteUserCommentRelationWhenDeletedPostDomainEventHandler> _logger;

        public DeleteUserCommentRelationWhenDeletedPostDomainEventHandler(
            IUserCommentRelationRepository userCommentRelationRepository,
            ILogger<DeleteUserCommentRelationWhenDeletedPostDomainEventHandler> logger)
        {
            _userCommentRelationRepository = userCommentRelationRepository ?? throw new ArgumentNullException(nameof(userCommentRelationRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(DeletedPostDomainEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("----- Handling DeletedPostDomainEvent: at {AppName} - ({@DomainEvent})", Program.AppName, notification);
            
            var relations = await _userCommentRelationRepository.GetRelationsByCommentIdsAsync(notification.CommentIds);
            relations.ForEach(r => _userCommentRelationRepository.Remove(r));
        }
    }
}
