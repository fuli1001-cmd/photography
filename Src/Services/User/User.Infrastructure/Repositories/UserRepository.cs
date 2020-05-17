using Photography.Services.User.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Text;
using Photography.Services.User.Infrastructure;
using Arise.DDD.Infrastructure;

namespace Photography.Services.User.Infrastructure.Repositories
{
    public class UserRepository : EfRepository<Domain.AggregatesModel.UserAggregate.User, UserContext>, IUserRepository
    {
        public UserRepository(UserContext context) : base(context)
        {

        }
    }
}
