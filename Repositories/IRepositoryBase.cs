using System.Linq.Expressions;

namespace Repositories
{
    public interface IRepositoryBase<TEntity> where TEntity : class
    {
        Task CreateAsync(TEntity entity);
        Task Update(TEntity entity);
        Task Delete(TEntity entity);
        Task DeleteById(Guid id);
        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes);
        //Task<TEntity?> GetByIdAsync(Guid id);
        Task<List<TEntity>> GetAllAsync();
        Task SaveChangesAsync();
    }
}
