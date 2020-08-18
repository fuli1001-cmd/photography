using Arise.DDD.Infrastructure.Data;
using Photography.Services.User.Domain.AggregatesModel.FeedbackAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.User.Infrastructure.Repositories
{
    public class FeedbackRepository : EfRepository<Feedback, UserContext>, IFeedbackRepository
    {
        public FeedbackRepository(UserContext context) : base(context) { }
    }
}
