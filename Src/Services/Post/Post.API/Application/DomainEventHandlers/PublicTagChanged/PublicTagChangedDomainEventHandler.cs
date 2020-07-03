using MediatR;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.Domain.AggregatesModel.TagAggregate;
using Photography.Services.Post.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.DomainEventHandlers.PublicTagChanged
{
    public class PublicTagChangedDomainEventHandler : INotificationHandler<PublicTagChangedDomainEvent>
    {
        private readonly ITagRepository _tagRepository;
        private readonly ILogger<PublicTagChangedDomainEventHandler> _logger;

        public PublicTagChangedDomainEventHandler(
            ITagRepository tagRepository,
            ILogger<PublicTagChangedDomainEventHandler> logger)
        {
            _tagRepository = tagRepository ?? throw new ArgumentNullException(nameof(tagRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(PublicTagChangedDomainEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("----- Handling PublicTagChangedDomainEvent: at {AppName} - ({@DomainEvent})", Program.AppName, notification);

            // 处理本次应用的标签
            if (notification.AppliedTags.Count > 0)
            {
                var appliedTags = await _tagRepository.GetPublicTagsByNames(notification.AppliedTags);
                notification.AppliedTags.ForEach(name =>
                {
                    var tag = appliedTags.SingleOrDefault(t => t.Name.ToLower() == name.ToLower());
                    if (tag == null)
                    {
                        tag = new Tag(name);
                        _tagRepository.Add(tag);
                    }
                    else
                    {
                        tag.IncreaseCount();
                    }
                });
            }

            // 处理本次去掉的标签
            if (notification.RemovedTags.Count > 0)
            {
                var removedTags = await _tagRepository.GetPublicTagsByNames(notification.RemovedTags);
                removedTags.ForEach(t =>
                {
                    t.DecreaseCount();

                    // 标签引用次数为0时，删掉标签
                    if (t.Count == 0)
                        _tagRepository.Remove(t);
                });
            }
        }
    }
}
