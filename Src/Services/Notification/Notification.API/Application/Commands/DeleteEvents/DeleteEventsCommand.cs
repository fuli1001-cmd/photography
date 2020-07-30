using MediatR;
using Photography.Services.Notification.Domain.AggregatesModel.EventAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Notification.API.Application.Commands.DeleteEvents
{
    /// <summary>
    /// 删除通知的命令
    /// EventIds和EventCategory任传一个，如果都传，则按照EventCategory删除
    /// </summary>
    public class DeleteEventsCommand : IRequest<bool>
    {
        /// <summary>
        /// 要删除的通知id列表
        /// </summary>
        public List<Guid> EventIds { get; set; }

        /// <summary>
        /// 要删除的通知类型，该类型下的所有通知都会被删除
        /// </summary>
        public EventCategory? EventCategory { get; set; }
    }
}
