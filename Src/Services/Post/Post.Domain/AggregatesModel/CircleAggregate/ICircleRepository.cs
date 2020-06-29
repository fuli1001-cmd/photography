using Arise.DDD.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Post.Domain.AggregatesModel.CircleAggregate
{
    public interface ICircleRepository : IRepository<Circle>
    {
        Task<Circle> GetCircleByNameAsync(string name);
    }
}
