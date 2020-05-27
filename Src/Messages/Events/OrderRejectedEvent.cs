﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Messages.Events
{
    public class OrderRejectedEvent : BaseEvent
    {
        public Guid UserId { get; set; }
        public Guid DealId { get; set; }
    }
}
