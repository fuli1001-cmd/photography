using Arise.DDD.Infrastructure.Data;
using Photography.Services.Order.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Order.Infrastructure.Repositories
{
    public class UserRepository : EfRepository<User, OrderContext>, IUserRepository
    {
        public UserRepository(OrderContext context) : base(context)
        {

        }
    }
}
