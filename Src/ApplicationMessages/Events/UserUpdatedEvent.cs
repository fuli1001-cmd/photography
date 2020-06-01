using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationMessages.Events
{
    public class UserUpdatedEvent : BaseEvent
    {
        public Guid UserId { get; set; }

        public string NickName { get; set; }

        public string Avatar { get; set; }

        public int? UserType { get; set; }
    }
}
