using System.Collections.Generic;
using System.Threading.Tasks;

namespace Photography.Services.Post.Domain.Seedwork
{
    public interface IRepository<T> where T : Entity, IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }

        Task<T> GetByIdAsync(int id);
        Task<List<T>> ListAllAsync();
        Task<List<T>> ListAsync(ISpecification<T> spec);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
    }
}
