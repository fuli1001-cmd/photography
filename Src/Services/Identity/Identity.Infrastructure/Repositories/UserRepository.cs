using Photography.Services.Identity.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Text;
using Arise.DDD.Infrastructure.Data.EF;
using Photography.Services.Identity.Infrastructure.EF;

namespace Photography.Services.Identity.Infrastructure.Repositories
{
    public class UserRepository : EfRepository<User, IdentityContext>, IUserRepository
    {
        public UserRepository(IdentityContext context) : base(context)
        {

        }
    }
}
