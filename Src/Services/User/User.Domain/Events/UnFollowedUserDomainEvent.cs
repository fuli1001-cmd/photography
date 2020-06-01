using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.User.Domain.Events
{
    public class UnFollowedUserDomainEvent : INotification
    {
        public Guid FollowerId { get; }
        public Guid FollowedUserId { get; }

        public UnFollowedUserDomainEvent(Guid followerId, Guid followedUserId)
        {
            FollowerId = followerId;
            FollowedUserId = followedUserId;
        }
    }
}
