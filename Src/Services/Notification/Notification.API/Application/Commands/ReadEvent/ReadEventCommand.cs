using MediatR;
using Photography.Services.Notification.Domain.AggregatesModel.EventAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Notification.API.Application.Commands.ReadEvent
{
    /// <summary>
    /// 标记某类别的事件为已读
    /// </summary>
    public class ReadEventCommand : IRequest<bool>
    {
        /// <summary>
        /// 事件类别
        /// </summary>
        public EventCategory EventCategory { get; set; }
    }
}
