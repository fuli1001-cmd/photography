using MediatR;
using Photography.Services.Notification.Domain.AggregatesModel.EventAggregate;
using Photography.Services.Notification.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Notification.API.Application.Commands.ConfigurePush
{
    public class ConfigurePushCommand : IRequest<bool>
    {
        public EventCategory EventCategory { get; set; }

        public PushSetting PushSetting { get; set; }
    }
}
