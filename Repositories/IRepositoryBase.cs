using System.Linq.Expressions;

namespace Repositories
{
    public interface IRepositoryBase<TEntity> where TEntity : class
    {
        Task CreateAsync(TEntity entity);
        Task Update(TEntity entity);
        Task Delete(TEntity entity);
        Task DeleteById(Guid id);
        Task<List<TEntity>> GetAllAsync(params Expression<Func<TEntity, object>>[] includes);
        Task SaveChangesAsync();
    }
}
