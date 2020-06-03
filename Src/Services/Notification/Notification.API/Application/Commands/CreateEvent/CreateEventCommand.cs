using MediatR;
using Photography.Services.Notification.Domain.AggregatesModel.EventAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Notification.API.Application.Commands.CreateEvent
{
    public class CreateEventCommand : IRequest<bool>
    {
        // 事件发起人id
        public Guid FromUserId { get; set; }

        // 事件接收人id
        public Guid ToUserId { get; set; }

        // 事件类型
        public EventType EventType { get; set; }

        public Guid? PostId { get; set; }

        public Guid? CommentId { get; set; }

        public string CommentText { get; set; }
    }
}
