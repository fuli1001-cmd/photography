﻿using Arise.DDD.Infrastructure.Data;
using Photography.Services.Notification.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Notification.Infrastructure.Repositories
{
    public class UserRepository : EfRepository<User, NotificationContext>, IUserRepository
    {
        public UserRepository(NotificationContext context) : base(context)
        {

        }
    }
}
