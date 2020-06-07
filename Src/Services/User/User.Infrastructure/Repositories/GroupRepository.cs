using Arise.DDD.Infrastructure;
using Photography.Services.User.Domain.AggregatesModel.GroupAggregate;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Photography.Services.User.Infrastructure.Repositories
{
    public class GroupRepository : EfRepository<Domain.AggregatesModel.GroupAggregate.Group, UserContext>, IGroupRepository
    {
        public GroupRepository(UserContext context) : base(context)
        {

        }
    }
}
