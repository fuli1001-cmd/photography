﻿using NServiceBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Messages.Events
{
    public class BaseEvent : IEvent
    {
        public Guid Id { get; set; }

        public BaseEvent()
        {
            Id = Guid.NewGuid();
        }
    }
}
